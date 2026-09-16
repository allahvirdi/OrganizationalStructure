using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Application.Authorization;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// هندلر احراز هویت BFF: کوکی نشست HttpOnly را به ClaimsPrincipal معتبر تبدیل می‌کند.
/// </summary>
/// <remarks>
/// مرورگر فقط شناسه نشست را دارد؛ توکن‌های IAM سمت سرور می‌مانند.
/// اعتبارسنجی با کش کوتاه و fail-closed انجام می‌شود (در دسترس‌نبودن IAM یعنی رد درخواست).
/// </remarks>
public sealed class BffSessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// نام Scheme احراز هویت.
    /// </summary>
    public const string SchemeName = "Bff";

    /// <summary>
    /// نام کوکی نشست.
    /// </summary>
    public const string SessionCookieName = "orgstructure_session";

    private readonly IBffSessionStore _sessions;
    private readonly IIamClient _iam;
    private readonly IClock _clock;
    private readonly IamOptions _options;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public BffSessionAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IBffSessionStore sessions,
        IIamClient iam,
        IClock clock,
        IOptions<IamOptions> iamOptions)
        : base(options, logger, encoder)
    {
        _sessions = sessions;
        _iam = iam;
        _clock = clock;
        _options = iamOptions.Value;
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(SessionCookieName, out var sessionId)
            || string.IsNullOrWhiteSpace(sessionId))
        {
            return AuthenticateResult.NoResult();
        }

        var session = await _sessions.GetAsync(sessionId);
        if (session is null)
        {
            return AuthenticateResult.Fail("نشست معتبر نیست.");
        }

        var cacheTtl = TimeSpan.FromSeconds(
            _options.ValidationCacheSeconds <= 0 ? 60 : _options.ValidationCacheSeconds);

        if (_clock.UtcNow - session.ValidatedAt > cacheTtl)
        {
            var validation = await _iam.ValidateAsync(session.AccessToken);
            if (!validation.IsValid)
            {
                await _sessions.RemoveAsync(sessionId);
                return AuthenticateResult.Fail("توکن نامعتبر است.");
            }

            var scope = session.VisibleOrganizationIds;
            var refreshed = await RefreshScopeAsync(session.AccessToken, validation.OrganizationId);
            if (refreshed.Count > 0)
            {
                scope = refreshed;
            }

            session = session with
            {
                ValidatedAt = _clock.UtcNow,
                UserId = validation.UserId ?? session.UserId,
                TenantId = validation.TenantId ?? session.TenantId,
                OrganizationId = validation.OrganizationId ?? session.OrganizationId,
                Roles = validation.Roles.Count > 0 ? validation.Roles : session.Roles,
                Permissions = validation.Permissions.Count > 0 ? validation.Permissions : session.Permissions,
                VisibleOrganizationIds = scope
            };
            await _sessions.SaveAsync(session);
        }

        var claims = new List<Claim>();
        if (!string.IsNullOrWhiteSpace(session.UserId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, session.UserId));
            claims.Add(new Claim(ClaimNames.UserId, session.UserId));
        }

        if (!string.IsNullOrWhiteSpace(session.TenantId))
        {
            claims.Add(new Claim(ClaimNames.TenantId, session.TenantId));
        }

        if (!string.IsNullOrWhiteSpace(session.OrganizationId))
        {
            claims.Add(new Claim(ClaimNames.OrganizationId, session.OrganizationId));
        }

        foreach (var role in session.Roles.Distinct(StringComparer.Ordinal))
        {
            claims.Add(new Claim(ClaimNames.Role, role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in session.Permissions.Distinct(StringComparer.Ordinal))
        {
            claims.Add(new Claim("permission", permission));
        }

        foreach (var organizationId in session.VisibleOrganizationIds.Distinct())
        {
            claims.Add(new Claim(ClaimNames.OrganizationScope, organizationId.ToString()));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return AuthenticateResult.Success(ticket);
    }

    /// <summary>
    /// بازیابی Scope از درخت IAM؛ در صورت شکست، مجموعه قبلی حفظ می‌شود.
    /// </summary>
    private async Task<IReadOnlyList<Guid>> RefreshScopeAsync(
        string accessToken,
        string? organizationId)
    {
        if (!Guid.TryParse(organizationId, out var orgId))
        {
            return Array.Empty<Guid>();
        }

        var tree = await _iam.GetOrganizationTreeAsync(accessToken);
        var scope = Application.Authorization.OrganizationScope.ComputeScope(tree, orgId);
        return scope.ToArray();
    }
}