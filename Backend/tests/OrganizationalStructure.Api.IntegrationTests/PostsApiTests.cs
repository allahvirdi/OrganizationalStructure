using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی Endpointهای پست (چرخه کامل HTTP + پایگاه داده).
/// </summary>
public sealed class PostsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public PostsApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static object NewPostPayload(Guid organizationId, string code, Guid? parentId = null) => new
    {
        organizationId,
        code,
        title = $"عنوان {code}",
        description = (string?)null,
        parentId
    };

    /// <summary>
    /// ایجاد پست معتبر باید ۲۰۱ برگرداند و قابل دریافت باشد.
    /// </summary>
    [Fact]
    public async Task Create_ValidPost_ShouldReturn201AndBeRetrievable()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/posts",
            NewPostPayload(organizationId, "MGR-001"));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await createResponse.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBe(Guid.Empty);

        var getResponse = await client.GetAsync($"/api/v1/posts/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// کد تکراری در سازمان باید ۴۰۹ برگرداند.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateCode_ShouldReturn409()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        var first = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, "DUP-001"));
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, "DUP-001"));
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// دریافت پست ناموجود باید ۴۰۴ برگرداند.
    /// </summary>
    [Fact]
    public async Task GetById_MissingPost_ShouldReturn404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/posts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// زیرشاخه باید ساختار درختی والد/فرزند را برگرداند.
    /// </summary>
    [Fact]
    public async Task Subtree_ParentWithChild_ShouldReturnTree()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        var parentResponse = await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "PAR-001"));
        var parentId = await parentResponse.Content.ReadFromJsonAsync<Guid>();

        var childResponse = await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "CHD-001", parentId));
        childResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var subtreeResponse = await client.GetAsync($"/api/v1/posts/{parentId}/subtree");
        subtreeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await subtreeResponse.Content.ReadAsStringAsync();
        body.Should().Contain("CHD-001");
    }

    /// <summary>
    /// جابجایی چرخه‌ای باید ۴۰۹ برگرداند.
    /// </summary>
    [Fact]
    public async Task Move_CreatingCycle_ShouldReturn409()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        var parentResponse = await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "P1-001"));
        var parentId = await parentResponse.Content.ReadFromJsonAsync<Guid>();

        var childResponse = await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "C1-001", parentId));
        var childId = await childResponse.Content.ReadFromJsonAsync<Guid>();

        var moveResponse = await client.PostAsJsonAsync(
            $"/api/v1/posts/{parentId}/move",
            new { newParentId = (Guid?)childId });

        moveResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// جستجو باید نتیجه صفحه‌بندی‌شده برگرداند.
    /// </summary>
    [Fact]
    public async Task Search_WithFilter_ShouldReturnPagedResult()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, "S-001"));

        var response = await client.GetAsync(
            $"/api/v1/posts?organizationId={organizationId}&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("S-001");
    }
}