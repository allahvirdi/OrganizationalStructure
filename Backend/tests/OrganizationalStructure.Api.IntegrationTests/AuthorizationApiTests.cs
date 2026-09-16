using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Infrastructure.Persistence;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی تصریح‌دهی: دسترسی خارج از Scope باید ۴۰۳ دهد.
/// </summary>
public sealed class AuthorizationApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public AuthorizationApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// بذر مستقیم پست خارج از Scope (دور زدن API که Scope را enforce می‌کند).
    /// </summary>
    private async Task<Guid> SeedOutOfScopePostAsync()
    {
        var options = new DbContextOptionsBuilder<OrganizationalStructureDbContext>()
            .UseSqlServer(_factory.ConnectionString)
            .Options;

        var protector = new AesPiiProtector(
            Options.Create(new PiiEncryptionOptions { Key = TestKeys.Pii }));

        using var db = new OrganizationalStructureDbContext(
            options,
            new FixedTenantContext(TestAuthHandler.TestTenantId),
            protector);

        var post = Post.Create(
            Guid.NewGuid(),
            TestAuthHandler.TestTenantId,
            Guid.NewGuid(),
            $"OOS-{Guid.NewGuid():N}"[..12],
            "پست خارج از Scope",
            null,
            null,
            DateTimeOffset.UtcNow);

        db.Posts.Add(post);
        await db.SaveChangesAsync();
        return post.Id;
    }

    /// <summary>
    /// دریافت پست خارج از Scope باید ۴۰۳ برگرداند (نه ۴۰۴).
    /// </summary>
    [Fact]
    public async Task GetById_OutOfScope_ShouldReturn403()
    {
        var client = _factory.CreateClient();
        var postId = await SeedOutOfScopePostAsync();

        var response = await client.GetAsync($"/api/v1/posts/{postId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// ایجاد پست خارج از Scope باید ۴۰۳ برگرداند.
    /// </summary>
    [Fact]
    public async Task Create_OutOfScope_ShouldReturn403()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/posts", new
        {
            organizationId = Guid.NewGuid(),
            code = "OOS-001",
            title = "t",
            description = (string?)null,
            parentId = (Guid?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// جستجو نباید پست خارج از Scope را برگرداند.
    /// </summary>
    [Fact]
    public async Task Search_ShouldNotReturnOutOfScopePosts()
    {
        var client = _factory.CreateClient();
        var postId = await SeedOutOfScopePostAsync();

        var response = await client.GetAsync("/api/v1/posts?page=1&pageSize=100");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain(postId.ToString());
    }
}