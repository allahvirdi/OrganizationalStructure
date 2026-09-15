using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی Endpointهای پرسنل و انتساب (شامل رفت‌وبرگشت رمزنگاری PII).
/// </summary>
public sealed class EmployeesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public EmployeesApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static object NewEmployeePayload(string code) => new
    {
        personnelCode = code,
        firstName = "علی",
        lastName = "رضایی",
        nationalCode = "0012345678",
        mobile = "09120000000",
        birthDate = "1981-08-03",
        serviceYears = 12,
        serviceMonths = 3,
        pezhvakMobile = "09190000000",
        userId = (Guid?)null
    };

    private static object NewPostPayload(Guid organizationId, string code) => new
    {
        organizationId,
        code,
        title = $"عنوان {code}",
        description = (string?)null,
        parentId = (Guid?)null,
        hasSigningAuthority = false
    };

    /// <summary>
    /// ثبت پرسنل باید ۲۰۱ برگرداند و داده حساس باید سالم برگردد (رمزگشایی شفاف).
    /// </summary>
    [Fact]
    public async Task Create_ValidEmployee_ShouldReturn201WithReadablePii()
    {
        var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/v1/employees", NewEmployeePayload("00000011"));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync($"/api/v1/employees/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await getResponse.Content.ReadAsStringAsync();
        body.Should().Contain("علی").And.Contain("0012345678");
    }

    /// <summary>
    /// کد تکراری باید ۴۰۹ برگرداند.
    /// </summary>
    [Fact]
    public async Task Create_DuplicateCode_ShouldReturn409()
    {
        var client = _factory.CreateClient();

        var first = await client.PostAsJsonAsync("/api/v1/employees", NewEmployeePayload("00000012"));
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/v1/employees", NewEmployeePayload("00000012"));
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// چرخه انتساب و پایان آن باید کار کند.
    /// </summary>
    [Fact]
    public async Task Assign_ThenEnd_ShouldWork()
    {
        var client = _factory.CreateClient();
        var organizationId = Guid.NewGuid();

        var postResponse = await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "EA-001"));
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var empResponse = await client.PostAsJsonAsync(
            "/api/v1/employees", NewEmployeePayload("00000013"));
        var employeeId = await empResponse.Content.ReadFromJsonAsync<Guid>();

        var assignResponse = await client.PostAsJsonAsync(
            $"/api/v1/employees/{employeeId}/posts",
            new { postId, fromDate = (string?)null, toDate = (string?)null, isPrimary = true });
        assignResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var postsResponse = await client.GetAsync($"/api/v1/employees/{employeeId}/posts");
        postsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        (await postsResponse.Content.ReadAsStringAsync()).Should().Contain("EA-001");

        var endResponse = await client.SendAsync(new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/v1/employees/{employeeId}/posts/{postId}")
        {
            Content = JsonContent.Create(new { endDate = "2026-09-30" })
        });
        endResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// انتساب اصلی دوم باید ۴۰۹ برگرداند.
    /// </summary>
    [Fact]
    public async Task Assign_SecondPrimary_ShouldReturn409()
    {
        var client = _factory.CreateClient();
        var organizationId = Guid.NewGuid();

        var post1 = await (await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "EB-001")))
            .Content.ReadFromJsonAsync<Guid>();
        var post2 = await (await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "EB-002")))
            .Content.ReadFromJsonAsync<Guid>();
        var employeeId = await (await client.PostAsJsonAsync(
            "/api/v1/employees", NewEmployeePayload("00000014")))
            .Content.ReadFromJsonAsync<Guid>();

        var first = await client.PostAsJsonAsync(
            $"/api/v1/employees/{employeeId}/posts",
            new { postId = post1, fromDate = (string?)null, toDate = (string?)null, isPrimary = true });
        first.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var second = await client.PostAsJsonAsync(
            $"/api/v1/employees/{employeeId}/posts",
            new { postId = post2, fromDate = (string?)null, toDate = (string?)null, isPrimary = true });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// پرسنل پست باید از مسیر posts قابل دریافت باشد.
    /// </summary>
    [Fact]
    public async Task GetPostEmployees_AfterAssign_ShouldContainEmployee()
    {
        var client = _factory.CreateClient();
        var organizationId = Guid.NewGuid();

        var postId = await (await client.PostAsJsonAsync(
            "/api/v1/posts", NewPostPayload(organizationId, "EC-001")))
            .Content.ReadFromJsonAsync<Guid>();
        var employeeId = await (await client.PostAsJsonAsync(
            "/api/v1/employees", NewEmployeePayload("00000015")))
            .Content.ReadFromJsonAsync<Guid>();

        await client.PostAsJsonAsync(
            $"/api/v1/employees/{employeeId}/posts",
            new { postId, fromDate = (string?)null, toDate = (string?)null, isPrimary = false });

        var response = await client.GetAsync($"/api/v1/posts/{postId}/employees");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("00000015");
    }
}