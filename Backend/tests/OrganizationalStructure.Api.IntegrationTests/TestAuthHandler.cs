using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganizationalStructure.API.Security;
using OrganizationalStructure.Domain.Abstractions;

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
    /// شناسه سازمان ثابت تست (Scope همه تست‌ها).
    /// </summary>
    public static readonly Guid TestOrganizationId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    /// <summary>
    /// شناسه سازمان فرزند ثابت تست (داخل Scope کاربر).
    /// </summary>
    public static readonly Guid TestChildOrganizationId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    /// <summary>
    /// شناسه سازمان نوه ثابت تست (داخل Scope کاربر).
    /// </summary>
    public static readonly Guid TestGrandchildOrganizationId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    /// <summary>
    /// شناسه سازمان خارج از Scope تست (نباید در گزینه‌های سازمان ظاهر شود).
    /// </summary>
    public static readonly Guid TestForeignOrganizationId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    /// <summary>
    /// نام سازمان خود کاربر در تست.
    /// </summary>
    public const string TestOrganizationName = "شرکت آزمون";

    /// <summary>
    /// نام سازمان فرزند در تست.
    /// </summary>
    public const string TestChildOrganizationName = "واحد فناوری";

    /// <summary>
    /// نام سازمان نوه در تست.
    /// </summary>
    public const string TestGrandchildOrganizationName = "گروه توسعه";

    /// <summary>
    /// نام سازمان خارج از محدوده در تست.
    /// </summary>
    public const string TestForeignOrganizationName = "سازمان خارج از محدوده";

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
            new(ClaimNames.TenantId, TestTenantId.ToString()),
            new(ClaimNames.OrganizationId, TestOrganizationId.ToString())
        };

        claims.AddRange(ScopeClaims());

        if (Request.Headers.TryGetValue("X-Test-Role", out var role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }
        else
        {
            foreach (var permission in AuthorizationPolicies.All)
            {
                claims.Add(new Claim(AuthorizationPolicies.PermissionClaimType, permission));
            }
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }

    /// <summary>
    /// ادعاهای محدوده سازمانی تست: خود سازمان + فرزند + نوه (سازمان خارج از محدوده عمداً غایب است).
    /// </summary>
    private static IEnumerable<Claim> ScopeClaims()
    {
        yield return new Claim(ClaimNames.OrganizationScope, TestOrganizationId.ToString());
        yield return new Claim(ClaimNames.OrganizationScope, TestChildOrganizationId.ToString());
        yield return new Claim(ClaimNames.OrganizationScope, TestGrandchildOrganizationId.ToString());

        foreach (var organization in new[]
        {
            new OrganizationReference(TestOrganizationId, TestOrganizationName, "TEST-ROOT", null, 0),
            new OrganizationReference(TestChildOrganizationId, TestChildOrganizationName, "TEST-CHILD", TestOrganizationId, 1),
            new OrganizationReference(TestGrandchildOrganizationId, TestGrandchildOrganizationName, "TEST-GRAND", TestChildOrganizationId, 2)
        })
        {
            yield return new Claim(
                ClaimNames.OrganizationScopeNode,
                OrganizationScopeClaim.Serialize(organization));
        }
    }
}