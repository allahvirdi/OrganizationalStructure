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

    /// <summary>هر دو نقش بدون مجوز صریح می‌توانند والد را جستجو و فرزند را در همان سازمان ثبت کنند.</summary>
    [Theory]
    [InlineData("SystemAdmin")]
    [InlineData("OrganizationStructureAdmin")]
    public async Task Create_WithAdminRoleAndParent_ShouldPersistHierarchy(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = Guid.NewGuid().ToString("N");
        var parentResponse = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, code));
        parentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var parentId = await parentResponse.Content.ReadFromJsonAsync<Guid>();
        var search = await client.GetAsync($"/api/v1/posts?organizationId={organizationId}&searchTerm={code}");
        search.StatusCode.Should().Be(HttpStatusCode.OK);
        (await search.Content.ReadAsStringAsync()).Should().Contain(parentId.ToString());
        var response = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, Guid.NewGuid().ToString("N"), parentId));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        var detail = await client.GetFromJsonAsync<System.Text.Json.JsonElement>($"/api/v1/posts/{id}");
        detail.GetProperty("organizationId").GetGuid().Should().Be(organizationId);
        detail.GetProperty("parentId").GetGuid().Should().Be(parentId);
        var forbidden = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(Guid.NewGuid(), code));
        forbidden.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>نقش عادی و نام مشابه مدیر بدون مجوز ایجاد پذیرفته نمی‌شوند.</summary>
    [Theory]
    [InlineData("User")]
    [InlineData("organizationstructureadmin")]
    public async Task Create_WithoutAuthorizedRole_ShouldReturnForbidden(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        var response = await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(TestAuthHandler.TestOrganizationId, "DENIED"));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
    /// جستجو باید نتیجه صفحه‌بندیشده برگرداند.
    /// </summary>
    [Fact]
    public async Task Search_WithFilter_ShouldReturnPagedResult()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;

        await client.PostAsJsonAsync("/api/v1/posts", NewPostPayload(organizationId, "S-001"));

        var response = await client.GetAsync(
            $"/api/v1/posts?organizationId={organizationId}&searchTerm=S-001&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("S-001");
    }

    /// <summary>
    /// ثبت پست در سازمان زیرمجموعه (داخل Scope کاربر) باید مجاز باشد.
    /// </summary>
    [Fact]
    public async Task Create_InDescendantOrganization_ShouldReturnCreated()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/posts",
            NewPostPayload(TestAuthHandler.TestChildOrganizationId, Guid.NewGuid().ToString("N")));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}