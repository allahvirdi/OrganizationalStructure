using System.Net;
using System.Text;
using ClosedXML.Excel;
using FluentAssertions;
using OrganizationalStructure.Application.Import;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی دانلود قالب‌های ورود پرسنل.
/// </summary>
public sealed partial class ImportEmployeesApiTests
{
    /// <summary>
    /// قالب اکسل پرسنل باید هدرهای فارسی و ردیف نمونه داشته باشد.
    /// </summary>
    [Fact]
    public async Task GetEmployeesTemplate_Xlsx_ShouldReturnPersianTemplate()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/import/employees/template?format=xlsx");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType
            .Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        using var stream = await response.Content.ReadAsStreamAsync();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        worksheet.Cell(1, 1).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[0]);
        worksheet.Cell(1, 4).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[3]);
        worksheet.Cell(1, 6).GetString().Should().Be(EmployeesImportHeaders.ColumnHeaders[5]);
        worksheet.Cell(2, 1).GetString().Should().Be("00000001");
    }

    /// <summary>
    /// قالب CSV پرسنل باید هدر فارسی و ردیف نمونه داشته باشد.
    /// </summary>
    [Fact]
    public async Task GetEmployeesTemplate_Csv_ShouldReturnPersianTemplate()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/import/employees/template?format=csv");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/csv");

        var body = await response.Content.ReadAsStringAsync();
        var text = body.TrimStart('\uFEFF');

        text.Should().Contain(EmployeesImportHeaders.CsvHeaderLine);
        text.Should().Contain("00000001,علی,رضایی");
    }
}