using ClosedXML.Excel;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// ساخت قالب فایل اکسل ورود پرسنل.
/// </summary>
public sealed partial class EmployeesExcelParser
{
    /// <summary>
    /// ساخت فایل قالب نمونه با هدر فارسی و دو ردیف نمونه.
    /// </summary>
    /// <returns>آرایه بایت‌های فایل اکسل قالب</returns>
    public static byte[] BuildTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("پرسنل");

        SetRow(worksheet, 1, EmployeesImportHeaders.ColumnHeaders);
        SetRow(worksheet, 2, "00000001", "علی", "رضایی", "0013542419", "09120000000", "1360/05/12", "12", "3", "09190000000");
        SetRow(worksheet, 3, "00000002", "مریم", "احمدی", "0013542427", "09121111111", "", "", "", "");

        var widths = new[] { 14, 20, 20, 14, 15, 20, 10, 10, 15 };
        for (var i = 0; i < widths.Length; i++)
        {
            worksheet.Column(i + 1).Width = widths[i];
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// نوشتن مقادیر یک ردیف در شیت.
    /// </summary>
    private static void SetRow(IXLWorksheet worksheet, int rowNumber, params string[] values)
    {
        for (var i = 0; i < values.Length; i++)
        {
            worksheet.Cell(rowNumber, i + 1).Value = values[i];
        }
    }
}