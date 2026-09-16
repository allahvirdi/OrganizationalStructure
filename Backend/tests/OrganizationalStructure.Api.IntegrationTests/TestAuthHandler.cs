using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganizationalStructure.API.Security;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// هندلر احراز هویت تست: principal با تمام دسترسی‌ها صادر می‌کند (فقط در تست).
/// </summary>
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// نام Scheme تست.
    /// </summary>
    public const string SchemeName = "Test";

    /// <summary>
    /// شناسه مستأجر ثابت تست (هماهنگ با سراسر تست‌ها).
    /// </summary>
    public static readonly Guid TestTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "22222222-2222-2222-2222-222222222222"),
            new(ClaimNames.UserId, "22222222-2222-2222-2222-222222222222"),
            new(ClaimNames.TenantId, TestTenantId.ToString())
        };

        foreach (var permission in AuthorizationPolicies.All)
        {
            claims.Add(new Claim(AuthorizationPolicies.PermissionClaimType, permission));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}