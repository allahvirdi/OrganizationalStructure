using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.API.Controllers;

/// <summary>
/// احراز هویت BFF: ورود/خروج/نشست جاری (مرورگر هرگز توکن IAM را نمی‌بیند).
/// </summary>
[Route("api/v1/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IIamClient _iam;
    private readonly IBffSessionStore _sessions;
    private readonly IClock _clock;
    private readonly IamOptions _options;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<VerifyMfaRequest> _mfaValidator;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public AuthController(
        IIamClient iam,
        IBffSessionStore sessions,
        IClock clock,
        Microsoft.Extensions.Options.IOptions<IamOptions> options,
        IValidator<LoginRequest> loginValidator,
        IValidator<VerifyMfaRequest> mfaValidator)
    {
        _iam = iam;
        _sessions = sessions;
        _clock = clock;
        _options = options.Value;
        _loginValidator = loginValidator;
        _mfaValidator = mfaValidator;
    }

    /// <summary>
    /// ورود با نام کاربری و رمز عبور.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResultDto>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation.Failed",
                Detail = string.Join("؛ ", validation.Errors.Select(e => e.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest
            });
        }

        var login = await _iam.LoginAsync(
            new IamLoginRequest
            {
                UserName = request.UserName,
                Password = request.Password,
                RememberMe = request.RememberMe
            },
            cancellationToken);

        if (!login.IsSuccess)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Auth.LoginFailed",
                Detail = login.Error ?? "ورود ناموفق بود.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        if (login.RequiresMfa)
        {
            return Ok(new LoginResultDto(
                RequiresMfa: true,
                MfaChallengeToken: login.MfaChallengeToken,
                UserId: null,
                ExpiresAt: null));
        }

        return await CompleteLoginAsync(
            login.AccessToken!, login.RefreshToken!, cancellationToken);
    }

    /// <summary>
    /// تکمیل ورود با کد دومرحله‌ای.
    /// </summary>
    [HttpPost("verify-mfa")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResultDto>> VerifyMfa(
        [FromBody] VerifyMfaRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _mfaValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation.Failed",
                Detail = string.Join("؛ ", validation.Errors.Select(e => e.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest
            });
        }

        var login = await _iam.VerifyMfaAsync(
            new IamMfaRequest
            {
                MfaChallengeToken = request.MfaChallengeToken,
                Code = request.Code,
                RememberMe = request.RememberMe
            },
            cancellationToken);

        if (!login.IsSuccess || string.IsNullOrWhiteSpace(login.AccessToken))
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Auth.LoginFailed",
                Detail = login.Error ?? "تأیید دومرحله‌ای ناموفق بود.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return await CompleteLoginAsync(
            login.AccessToken, login.RefreshToken!, cancellationToken);
    }

    /// <summary>
    /// خروج: ابطال توکن و حذف نشست سمت‌سرور + کوکی.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(
                BffSessionAuthenticationHandler.SessionCookieName, out var sessionId)
            && !string.IsNullOrWhiteSpace(sessionId))
        {
            var session = await _sessions.GetAsync(sessionId, cancellationToken);
            if (session is not null && !string.IsNullOrWhiteSpace(session.RefreshToken))
            {
                await _iam.RevokeAsync(session.RefreshToken, cancellationToken);
            }

            await _sessions.RemoveAsync(sessionId, cancellationToken);
        }

        Response.Cookies.Delete(BffSessionAuthenticationHandler.SessionCookieName);
        return NoContent();
    }

    /// <summary>
    /// اطلاعات کاربر جاری از نشست معتبر.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserDto> Me()
    {
        var user = HttpContext.User;
        return Ok(new CurrentUserDto(
            user.FindFirst(ClaimNames.UserId)?.Value,
            user.FindFirst(ClaimNames.TenantId)?.Value,
            user.FindFirst(ClaimNames.OrganizationId)?.Value,
            user.FindAll(ClaimNames.Role).Select(c => c.Value).ToArray()));
    }

    private async Task<ActionResult<LoginResultDto>> CompleteLoginAsync(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken)
    {
        var validation = await _iam.ValidateAsync(accessToken, cancellationToken);
        if (!validation.IsValid)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Auth.LoginFailed",
                Detail = validation.Error ?? "توکن نامعتبر است.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var now = _clock.UtcNow;
        var minutes = _options.SessionMinutes <= 0 ? 30 : _options.SessionMinutes;
        var sessionId = Guid.NewGuid().ToString("N");

        await _sessions.SaveAsync(new BffSession
        {
            SessionId = sessionId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = now.AddMinutes(minutes),
            ValidatedAt = now,
            UserId = validation.UserId,
            TenantId = validation.TenantId,
            OrganizationId = validation.OrganizationId,
            Roles = validation.Roles,
            Permissions = validation.Permissions
        }, cancellationToken);

        Response.Cookies.Append(
            BffSessionAuthenticationHandler.SessionCookieName,
            sessionId,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = now.AddMinutes(minutes)
            });

        return Ok(new LoginResultDto(
            RequiresMfa: false,
            MfaChallengeToken: null,
            UserId: validation.UserId,
            ExpiresAt: now.AddMinutes(minutes)));
    }
}

/// <summary>
/// بدنه درخواست ورود.
/// </summary>
/// <param name="UserName">نام کاربری</param>
/// <param name="Password">رمز عبور</param>
/// <param name="RememberMe">نشست ماندگار</param>
public sealed record LoginRequest(string UserName, string Password, bool RememberMe);

/// <summary>
/// بدنه درخواست تأیید دومرحله‌ای.
/// </summary>
/// <param name="MfaChallengeToken">توکن چالش مرحله اول</param>
/// <param name="Code">کد تأیید</param>
/// <param name="RememberMe">نشست ماندگار</param>
public sealed record VerifyMfaRequest(string MfaChallengeToken, string Code, bool RememberMe);

/// <summary>
/// پاسخ ورود.
/// </summary>
/// <param name="RequiresMfa">نیاز به دومرحله‌ای</param>
/// <param name="MfaChallengeToken">توکن چالش (در صورت نیاز)</param>
/// <param name="UserId">شناسه کاربر (پس از تکمیل)</param>
/// <param name="ExpiresAt">انقضای نشست (پس از تکمیل)</param>
public sealed record LoginResultDto(
    bool RequiresMfa,
    string? MfaChallengeToken,
    string? UserId,
    DateTimeOffset? ExpiresAt);

/// <summary>
/// اطلاعات کاربر جاری.
/// </summary>
/// <param name="UserId">شناسه کاربر</param>
/// <param name="TenantId">شناسه مستأجر</param>
/// <param name="OrganizationId">شناسه سازمان</param>
/// <param name="Roles">نقش‌ها</param>
public sealed record CurrentUserDto(
    string? UserId,
    string? TenantId,
    string? OrganizationId,
    string[] Roles);

/// <summary>
/// اعتبارسنج درخواست ورود.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("نام کاربری الزامی است.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("رمز عبور الزامی است.");
    }
}

/// <summary>
/// اعتبارسنج درخواست تأیید دومرحله‌ای.
/// </summary>
public sealed class VerifyMfaRequestValidator : AbstractValidator<VerifyMfaRequest>
{
    /// <summary>
    /// تعریف قواعد اعتبارسنجی.
    /// </summary>
    public VerifyMfaRequestValidator()
    {
        RuleFor(x => x.MfaChallengeToken)
            .NotEmpty()
            .WithMessage("توکن چالش الزامی است.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("کد تأیید الزامی است.");
    }
}