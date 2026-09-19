using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.API.Security;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>All domain policies accept authenticated IAM SystemAdmin, not lookalike roles or anonymous claims.</summary>
public sealed class SystemAdminPolicyTests
{
    public static IEnumerable<object[]> Policies => AuthorizationPolicies.All.Select(policy => new object[] { policy });

    [Theory]
    [MemberData(nameof(Policies))]
    public async Task DomainPolicy_ShouldEnforceAuthenticatedRoleOrExactPermission(string policy)
    {
        using var services = new ServiceCollection()
            .AddLogging()
            .AddOrgAuthorization()
            .BuildServiceProvider();
        var authorization = services.GetRequiredService<IAuthorizationService>();

        async Task<bool> Allowed(string? role, string? permission, bool authenticated = true)
        {
            var claims = new List<Claim>();
            if (role is not null) claims.Add(new Claim(ClaimTypes.Role, role));
            if (permission is not null) claims.Add(new Claim(AuthorizationPolicies.PermissionClaimType, permission));
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticated ? "ValidatedIam" : null));
            return (await authorization.AuthorizeAsync(user, null, policy)).Succeeded;
        }

        (await Allowed("SystemAdmin", null)).Should().BeTrue();
        (await Allowed("User", policy)).Should().BeTrue();
        (await Allowed("User", null)).Should().BeFalse();
        (await Allowed("systemadmin", null)).Should().BeFalse();
        (await Allowed("SystemAdmin", null, false)).Should().BeFalse();
        (await Allowed(null, policy, false)).Should().BeFalse();
        (await Allowed(null, "Unrelated.Permission")).Should().BeFalse();
    }
}
