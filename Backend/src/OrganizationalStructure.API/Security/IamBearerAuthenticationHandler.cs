using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Application.Integration.Iam;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// هندلر احراز هویت Bearer برای مصرف‌کننده‌های ماشینی (توکن JWT معتبر IAM).
/// </summary>
/// <remarks>
/// اعتبارسنجی از طریق introspection سمت IAM انجام می‌شود (fail-closed)؛
/// پس از موفقیت، همان Claimها و Scope مدل BFF ساخته می‌شود تا Policyها یکسان اعمال شوند (ADR-012).
/// </remarks>
public sealed class IamBearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IIamClient _iam;
    private readonly OrganizationScopeResolver _scopeResolver;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public IamBearerAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IIamClient iam,
        OrganizationScopeResolver scopeResolver)
        : base(options, logger, encoder)
    {
        _iam = iam;
        _scopeResolver = scopeResolver;
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization)
            || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var token = authorization["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("توکن ارائه نشده است.");
        }

        var validation = await _iam.ValidateAsync(token);
        if (!validation.IsValid)
        {
            return AuthenticateResult.Fail("توکن نامعتبر است.");
        }

        var scope = await _scopeResolver.ResolveScopeAsync(token, validation.OrganizationId);
        if (scope.Count == 0)
        {
            return AuthenticateResult.Fail("محدوده سازمانی قابل تشخیص نیست.");
        }

        var claims = new List<Claim>();
        if (!string.IsNullOrWhiteSpace(validation.UserId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, validation.UserId));
            claims.Add(new Claim(ClaimNames.UserId, validation.UserId));
        }

        if (!string.IsNullOrWhiteSpace(validation.TenantId))
        {
            claims.Add(new Claim(ClaimNames.TenantId, validation.TenantId));
        }

        if (!string.IsNullOrWhiteSpace(validation.OrganizationId))
        {
            claims.Add(new Claim(ClaimNames.OrganizationId, validation.OrganizationId));
        }

        foreach (var role in validation.Roles.Distinct(StringComparer.Ordinal))
        {
            claims.Add(new Claim(ClaimNames.Role, role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var permission in validation.Permissions.Distinct(StringComparer.Ordinal))
        {
            claims.Add(new Claim("permission", permission));
        }

        foreach (var organization in scope)
        {
            claims.Add(new Claim(ClaimNames.OrganizationScope, organization.Id.ToString()));
            claims.Add(new Claim(
                ClaimNames.OrganizationScopeNode,
                OrganizationScopeClaim.Serialize(organization)));
        }

        var identity = new ClaimsIdentity(claims, AuthenticationSchemes.IamBearer);
        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, AuthenticationSchemes.IamBearer));
    }
}