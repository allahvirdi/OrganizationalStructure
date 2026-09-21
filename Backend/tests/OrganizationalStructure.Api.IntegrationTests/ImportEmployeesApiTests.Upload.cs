using System.Net;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی بارگذاری فایل پرسنل.
/// </summary>
public sealed partial class ImportEmployeesApiTests
{
    /// <summary>
    /// بارگذاری فایل اکسل معتبر پرسنل باید ۲۰۱ برگرداند و رکورد قابل جستجو باشد.
    /// </summary>
    [Fact]
    public async Task ImportEmployees_ValidExcel_ShouldReturn201AndPersistEmployee()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var file = CreateExcelFile(new[]
        {
            code, "علی", "رضایی", national, "09120000000", "1360/05/12", "12", "3", "09190000000"
        });

        var response = await client.PostAsync(
            "/api/v1/import/employees",
            CreateMultipart(organizationId, file));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadAsStringAsync()).Should().Contain("importedCount");

        var search = await client.GetAsync($"/api/v1/employees?personnelCode={code}");
        search.StatusCode.Should().Be(HttpStatusCode.OK);
        (await search.Content.ReadAsStringAsync()).Should().Contain(code);
    }

    /// <summary>
    /// بارگذاری فایل CSV معتبر پرسنل باید ۲۰۱ برگرداند.
    /// </summary>
    [Fact]
    public async Task ImportEmployees_ValidCsv_ShouldReturn201()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var row = string.Join(",", new[]
        {
            code, "مریم", "احمدی", national, "09121111111", "", "", "", ""
        });

        var file = CreateCsvContent(string.Join("\r\n", string.Join(",", Headers), row, string.Empty));

        var response = await client.PostAsync(
            "/api/v1/import/employees",
            CreateMultipart(organizationId, file, "پرسنل-آزمون.csv"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadAsStringAsync()).Should().Contain("importedCount");
    }

    /// <summary>
    /// پسوند نامعتبر برای بارگذاری پرسنل باید ۴۰۰ برگرداند.
    /// </summary>
    [Fact]
    public async Task ImportEmployees_InvalidExtension_ShouldReturn400()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var file = CreateExcelFile(new[]
        {
            code, "علی", "رضایی", national, "09120000000", "", "", "", ""
        });

        var response = await client.PostAsync(
            "/api/v1/import/employees",
            CreateMultipart(organizationId, file, "پرسنل-آزمون.xls"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// نقش بدون مجوز وارد کردن پرسنل نباید بتواند بارگذاری کند.
    /// </summary>
    [Fact]
    public async Task ImportEmployees_WithoutPermission_ShouldReturn403()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "User");
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = NextPersonnelCode();
        var national = MakeNationalCode(code + "0");

        var file = CreateExcelFile(new[]
        {
            code, "علی", "رضایی", national, "09120000000", "", "", "", ""
        });

        var response = await client.PostAsync(
            "/api/v1/import/employees",
            CreateMultipart(organizationId, file));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}