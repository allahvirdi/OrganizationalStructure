using System.Net;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using FluentAssertions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی Endpointهای بارگذاری ساختار از فایل اکسل.
/// </summary>
public sealed class ImportApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public ImportApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static ByteArrayContent CreateExcelFile(params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("ساختار پست‌ها");

        worksheet.Cell(1, 1).Value = "کد پست";
        worksheet.Cell(1, 2).Value = "عنوان پست";
        worksheet.Cell(1, 3).Value = "شرح";
        worksheet.Cell(1, 4).Value = "کد پست والد";

        for (var index = 0; index < rows.Length; index++)
        {
            var row = rows[index];
            for (var cellIndex = 0; cellIndex < row.Length; cellIndex++)
            {
                worksheet.Cell(index + 2, cellIndex + 1).Value = row[cellIndex];
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var content = new ByteArrayContent(stream.ToArray());
        content.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        return content;
    }

    private static MultipartFormDataContent CreateMultipart(Guid organizationId, ByteArrayContent fileContent, string fileName = "ساختار-آزمون.xlsx")
    {
        var multipart = new MultipartFormDataContent();
        multipart.Add(new StringContent(organizationId.ToString()), "organizationId");
        multipart.Add(fileContent, "file", fileName);
        return multipart;
    }

    /// <summary>
    /// بارگذاری فایل معتبر باید ۲۰۱ برگرداند و پست‌ها را قابل جستجو کند.
    /// </summary>
    [Fact]
    public async Task ImportPosts_ValidFile_ShouldReturn201AndPersistPosts()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var fileContent = CreateExcelFile(
            new[] { $"{code}-001", "مدیرعامل", "ریشه", "" },
            new[] { $"{code}-002", "معاون فنی", "", $"{code}-001" });

        var response = await client.PostAsync(
            "/api/v1/import/posts",
            CreateMultipart(organizationId, fileContent));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("2");

        var search = await client.GetAsync(
            $"/api/v1/posts?organizationId={organizationId}&searchTerm={code}");
        var searchBody = await search.Content.ReadAsStringAsync();
        searchBody.Should().Contain($"{code}-002");
    }

    /// <summary>
    /// فایل با پسوند نادرست باید ۴۰۰ برگرداند.
    /// </summary>
    [Fact]
    public async Task ImportPosts_InvalidExtension_ShouldReturn400()
    {
        var client = _factory.CreateClient();
        var organizationId = TestAuthHandler.TestOrganizationId;
        var fileContent = CreateExcelFile(new[] { "MGR-001", "مدیرعامل", "", "" });

        var response = await client.PostAsync(
            "/api/v1/import/posts",
            CreateMultipart(organizationId, fileContent, "ساختار-آزمون.xls"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// نقش بدون مجوز ایجاد پست نباید بتواند بارگذاری کند.
    /// </summary>
    [Fact]
    public async Task ImportPosts_WithoutPermission_ShouldReturn403()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "User");
        var organizationId = TestAuthHandler.TestOrganizationId;
        var fileContent = CreateExcelFile(new[] { "MGR-403", "مدیرعامل", "", "" });

        var response = await client.PostAsync(
            "/api/v1/import/posts",
            CreateMultipart(organizationId, fileContent));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// قالب نمونه باید با هدر فارسی و دو ردیف داده قابل پارس شدن برگردد.
    /// </summary>
    [Fact]
    public async Task GetTemplate_ShouldReturnValidPersianTemplate()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/import/posts/template");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType
            .Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        using var stream = await response.Content.ReadAsStreamAsync();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        worksheet.Cell(1, 1).GetString().Should().Be("کد پست");
        worksheet.Cell(1, 2).GetString().Should().Be("عنوان پست");
        worksheet.Cell(1, 3).GetString().Should().Be("شرح");
        worksheet.Cell(1, 4).GetString().Should().Be("کد پست والد");
        worksheet.Cell(2, 1).GetString().Should().Be("MGR-001");
        worksheet.Cell(3, 4).GetString().Should().Be("MGR-001");
    }
}