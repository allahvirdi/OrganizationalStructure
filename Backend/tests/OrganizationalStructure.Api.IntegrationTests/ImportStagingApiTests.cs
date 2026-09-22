using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using ClosedXML.Excel;
using FluentAssertions;
using OrganizationalStructure.Application.Import;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// تست‌های یکپارچگی اندپوینت‌های بارگذاری واسط (Staging) — DEC-030.
/// </summary>
public sealed partial class ImportStagingApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private static int _sequence = 80000000;

    private static readonly string[] Headers = EmployeesImportHeaders.ColumnHeaders;

    /// <summary>
    /// مقداردهی اولیه با کارخانه مشترک.
    /// </summary>
    public ImportStagingApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static string NextPersonnelCode()
    {
        return Interlocked.Increment(ref _sequence).ToString().PadLeft(8, '0');
    }

    private static string MakeNationalCode(string prefix9)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += (prefix9[i] - '0') * (10 - i);
        }

        var remainder = sum % 11;
        var checkDigit = remainder < 2 ? remainder : 11 - remainder;
        return prefix9 + checkDigit.ToString();
    }

    private static ByteArrayContent CreateExcelFile(params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("پرسنل");

        for (var i = 0; i < Headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = Headers[i];
        }

        for (var r = 0; r < rows.Length; r++)
        {
            var row = rows[r];
            for (var c = 0; c < row.Length && c < Headers.Length; c++)
            {
                worksheet.Cell(r + 2, c + 1).Value = row[c];
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var content = new ByteArrayContent(stream.ToArray());
        content.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        return content;
    }

    private static MultipartFormDataContent CreateMultipart(
        Guid organizationId,
        ByteArrayContent fileContent,
        string fileName = "پرسنل-آزمون.xlsx")
    {
        var multipart = new MultipartFormDataContent();
        multipart.Add(new StringContent(organizationId.ToString()), "organizationId");
        multipart.Add(fileContent, "file", fileName);
        return multipart;
    }
}