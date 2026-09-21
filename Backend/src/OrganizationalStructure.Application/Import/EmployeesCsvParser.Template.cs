using System.Text;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// ساخت قالب فایل CSV ورود پرسنل.
/// </summary>
public sealed partial class EmployeesCsvParser
{
    /// <summary>
    /// ساخت فایل قالب CSV با هدر فارسی و دو ردیف نمونه (UTF-8 با BOM).
    /// </summary>
    /// <returns>آرایه بایت‌های فایل CSV قالب</returns>
    public static byte[] BuildTemplate()
    {
        var content = string.Join(
            "\r\n",
            EmployeesImportHeaders.CsvHeaderLine,
            "00000001,علی,رضایی,0013542419,09120000000,1360/05/12,12,3,09190000000",
            "00000002,مریم,احمدی,0013542427,09121111111,,,",
            string.Empty);

        return Encoding.UTF8.GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(content))
            .ToArray();
    }
}