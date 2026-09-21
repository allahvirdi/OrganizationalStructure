using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// کمکی‌های مشترک پارسر فایل اکسل پرسنل.
/// </summary>
public sealed partial class EmployeesExcelParser
{
    /// <summary>
    /// بررسی خالی بودن ردیف بر اساس ستون‌های اجباری.
    /// </summary>
    /// <param name="code">کد پرسنلی</param>
    /// <param name="firstName">نام</param>
    /// <param name="lastName">نام خانوادگی</param>
    /// <param name="nationalCode">کد ملی</param>
    /// <returns>در صورت خالی بودن همه ستون‌های اجباری true</returns>
    private static bool IsEmptyRow(string? code, string firstName, string lastName, string? nationalCode)
    {
        return string.IsNullOrEmpty(code) &&
            string.IsNullOrEmpty(firstName) &&
            string.IsNullOrEmpty(lastName) &&
            string.IsNullOrEmpty(nationalCode);
    }

    /// <summary>
    /// ساخت نتیجه ناموفق خطای فایل.
    /// </summary>
    /// <param name="detail">توضیح علت</param>
    /// <returns>نتیجه ناموفق</returns>
    private static Result<IReadOnlyList<ImportEmployeeRowDto>> FileFailure(string detail)
    {
        return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(ImportErrors.InvalidFile(detail));
    }

    /// <summary>
    /// ساخت نتیجه ناموفق خطای ردیف.
    /// </summary>
    /// <param name="row">شماره ردیف داده</param>
    /// <param name="detail">توضیح علت</param>
    /// <returns>نتیجه ناموفق</returns>
    private static Result<IReadOnlyList<ImportEmployeeRowDto>> RowFailure(int row, string detail)
    {
        return Result<IReadOnlyList<ImportEmployeeRowDto>>.Failure(ImportErrors.EmployeeRowError(row, detail));
    }

    /// <summary>
    /// تبدیل رشته خالی به null.
    /// </summary>
    /// <param name="value">مقدار متنی</param>
    /// <returns>null در صورت خالی بودن، در غیر این صورت همان مقدار</returns>
    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }
}