using System.Text;
using ClosedXML.Excel;
using FluentAssertions;
using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// تست‌های پارسر و پردازش‌گر بارگذاری دسته‌جمعی پرسنل.
/// </summary>
public sealed partial class ImportEmployeesTests
{
    private readonly EmployeesExcelParser _excelParser = new();
    private readonly EmployeesCsvParser _csvParser = new();

    private static (OrganizationalStructureDbContext Db, TestClock Clock, TestCurrentUser User)
        CreateContext(IEnumerable<Guid> scope)
    {
        var tenantId = Guid.NewGuid();
        var db = TestDbContextFactory.Create(tenantId);
        return (db, new TestClock(), new TestCurrentUser(tenantId, scope));
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

    private static ImportEmployeeRowDto ValidRow(
        int rowNumber = 1,
        string personnelCode = "00000001")
    {
        return new ImportEmployeeRowDto(
            rowNumber,
            personnelCode,
            "علی",
            "رضایی",
            MakeNationalCode(personnelCode + "0"),
            "09120000000",
            "1360/05/12",
            "12",
            "3",
            "09190000000");
    }

    private static MemoryStream CreateExcelBytes(params string[][] rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("پرسنل");

        var headers = EmployeesImportHeaders.ColumnHeaders;

        for (var i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        for (var r = 0; r < rows.Length; r++)
        {
            var row = rows[r];
            for (var c = 0; c < row.Length && c < headers.Length; c++)
            {
                worksheet.Cell(r + 2, c + 1).Value = row[c];
            }
        }

        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreateCsvStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}