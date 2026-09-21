namespace OrganizationalStructure.Application.Import;

/// <summary>
/// سرستون‌های مشترک قالب و پارسرهای ورود پرسنل.
/// </summary>
public static class EmployeesImportHeaders
{
    /// <summary>
    /// نام ستون‌ها به ترتیب مورد انتظار در فایل اکسل و CSV.
    /// </summary>
    public static readonly string[] ColumnHeaders =
    {
        "کد پرسنلی",
        "نام",
        "نام خانوادگی",
        "کد ملی",
        "موبایل",
        "تاریخ تولد (شمسی)",
        "سال سابقه",
        "ماه سابقه",
        "موبایل پژواک"
    };

    /// <summary>
    /// خط هدر CSV ساخته‌شده از سرستون‌های مشترک.
    /// </summary>
    public static readonly string CsvHeaderLine = string.Join(",", ColumnHeaders);
}