using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی مسئولیت و اختیار (مدل Assignment-based).
/// </summary>
public sealed class AuthorityApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public AuthorityApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<Guid> CreatePostAsync(HttpClient client, string code)
    {
        var response = await client.PostAsJsonAsync("/api/v1/posts", new
        {
            organizationId = TestAuthHandler.TestOrganizationId,
            code,
            title = $"عنوان {code}",
            description = (string?)null,
            parentId = (Guid?)null
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<Guid>())!;
    }

    /// <summary>
    /// چرخه کامل مسئولیت: تعریف (کد خودکار)، انتساب، مشاهده در پست، پایان.
    /// </summary>
    [Fact]
    public async Task Responsibility_FullCycle_ShouldWork()
    {
        var client = _factory.CreateClient();
        var postId = await CreatePostAsync(client, "RA-001");

        // کد به‌صورت خودکار در بک‌اند تولید می‌شود (معادل شناسه)
        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/responsibilities",
            new { title = "مسئول دبیرخانه", description = (string?)null });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responsibilityId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var code = responsibilityId.ToString();

        var assignResponse = await client.PostAsJsonAsync(
            "/api/v1/responsibilities/assignments",
            new { responsibilityCode = code, postId, startDate = (string?)null, endDate = (string?)null });
        assignResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var assignmentId = await assignResponse.Content.ReadFromJsonAsync<Guid>();

        var postResponsibilities = await client.GetAsync($"/api/v1/posts/{postId}/responsibilities");
        postResponsibilities.StatusCode.Should().Be(HttpStatusCode.OK);
        (await postResponsibilities.Content.ReadAsStringAsync()).Should().Contain(code);

        var byCode = await client.GetAsync($"/api/v1/responsibilities/{code}");
        byCode.StatusCode.Should().Be(HttpStatusCode.OK);

        var endResponse = await client.PostAsJsonAsync(
            $"/api/v1/responsibilities/assignments/{assignmentId}/end",
            new { endDate = "2026-09-30" });
        endResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterEnd = await client.GetAsync($"/api/v1/posts/{postId}/responsibilities");
        (await afterEnd.Content.ReadAsStringAsync()).Should().NotContain($"\"responsibilityCode\":\"{code}\"");
    }

    /// <summary>
    /// انتساب تکراری جاری باید ۴۰۹ دهد.
    /// </summary>
    [Fact]
    public async Task Responsibility_DuplicateActiveAssignment_ShouldReturn409()
    {
        var client = _factory.CreateClient();
        var postId = await CreatePostAsync(client, "RA-002");

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/responsibilities",
            new { title = "t", description = (string?)null });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responsibilityId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var code = responsibilityId.ToString();

        var first = await client.PostAsJsonAsync(
            "/api/v1/responsibilities/assignments",
            new { responsibilityCode = code, postId, startDate = (string?)null, endDate = (string?)null });
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync(
            "/api/v1/responsibilities/assignments",
            new { responsibilityCode = code, postId, startDate = (string?)null, endDate = (string?)null });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// چرخه کامل اختیار و نمایش نشان امضا در درخت.
    /// </summary>
    [Fact]
    public async Task Authority_SigningBadge_ShouldAppearInSubtree()
    {
        var client = _factory.CreateClient();
        var postId = await CreatePostAsync(client, "RA-003");

        await client.PostAsJsonAsync(
            "/api/v1/authorities",
            new { code = "SIGNING_AUTHORITY", title = "اختیار امضا", description = (string?)null });

        var assign = await client.PostAsJsonAsync(
            "/api/v1/authorities/assignments",
            new { authorityCode = "SIGNING_AUTHORITY", postId, startDate = (string?)null, endDate = (string?)null });
        assign.StatusCode.Should().Be(HttpStatusCode.Created);

        var subtree = await client.GetAsync($"/api/v1/posts/{postId}/subtree");
        subtree.StatusCode.Should().Be(HttpStatusCode.OK);
        (await subtree.Content.ReadAsStringAsync()).Should().Contain("\"hasSigningAuthority\":true");

        var detail = await client.GetAsync($"/api/v1/posts/{postId}");
        (await detail.Content.ReadAsStringAsync()).Should().Contain("SIGNING_AUTHORITY");
    }

    /// <summary>
    /// عملیات روی موجودیت ناموجود باید ۴۰۴ دهد.
    /// </summary>
    [Fact]
    public async Task ResponsibilityAuthority_Missing_ShouldReturn404()
    {
        var client = _factory.CreateClient();
        var missingId = Guid.NewGuid();

        (await client.GetAsync("/api/v1/responsibilities/NOPE")).StatusCode
            .Should().Be(HttpStatusCode.NotFound);
        (await client.GetAsync("/api/v1/authorities/NOPE")).StatusCode
            .Should().Be(HttpStatusCode.NotFound);
        (await client.GetAsync($"/api/v1/posts/{missingId}/responsibilities")).StatusCode
            .Should().Be(HttpStatusCode.NotFound);
        (await client.GetAsync($"/api/v1/posts/{missingId}/authorities")).StatusCode
            .Should().Be(HttpStatusCode.NotFound);
    }
}