using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Employees;
using OrganizationalStructure.Application.Import.DTOs;
using OrganizationalStructure.Domain.Entities;
using OrganizationalStructure.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// قواعد اعتبارسنجی و ساخت موجودیت پرسنل در پردازش‌گر ورود دسته‌جمعی.
/// </summary>
public sealed partial class ImportEmployeesCommandHandler
{
    /// <summary>
    /// الگوی اعتبارسنجی کد پرسنلی.
    /// </summary>
    private static readonly Regex PersonnelCodeRegex = new("^[0-9]{8}$", RegexOptions.Compiled);

    /// <summary>
    /// الگوی اعتبارسنجی شماره همراه ایرانی.
    /// </summary>
    private static readonly Regex IranianMobileRegex = new("^09[0-9]{9}$", RegexOptions.Compiled);

    /// <summary>
    /// اعتبارسنجی و ساخت موجودیت پرسنل از یک ردیف فایل.
    /// </summary>
    /// <param name="row">ردیف پارس‌شده</param>
    /// <param name="tenantId">شناسه مستأجر</param>
    /// <param name="organizationId">شناسه سازمان</param>
    /// <param name="existingCodes">کدهای پرسنلی موجود در دیتابیس</param>
    /// <returns>نتیجه شامل موجودیت معتبر یا خطا</returns>
    private Result<Employee> BuildEmployee(
        ImportEmployeeRowDto row,
        Guid tenantId,
        Guid organizationId,
        HashSet<string> existingCodes)
    {
        if (!PersonnelCodeRegex.IsMatch(row.PersonnelCode))
        {
            return RowFailure(row.RowNumber, "کد پرسنلی باید دقیقاً ۸ رقم باشد.");
        }

        if (existingCodes.Contains(row.PersonnelCode))
        {
            return Result<Employee>.Failure(EmployeeErrors.DuplicatePersonnelCode(row.PersonnelCode));
        }

        if (row.FirstName.Length > 200)
        {
            return RowFailure(row.RowNumber, "نام بیش از ۲۰۰ کاراکتر است.");
        }

        if (row.LastName.Length > 200)
        {
            return RowFailure(row.RowNumber, "نام خانوادگی بیش از ۲۰۰ کاراکتر است.");
        }

        if (!IranianNationalCodeValidator.IsValid(row.NationalCode))
        {
            return RowFailure(row.RowNumber, "کد ملی معتبر نیست.");
        }

        var mobile = row.Mobile;
        if (string.IsNullOrWhiteSpace(mobile))
        {
            return RowFailure(row.RowNumber, "شماره همراه الزامی است.");
        }

        if (!IranianMobileRegex.IsMatch(mobile!))
        {
            return RowFailure(row.RowNumber, "فرمت شماره همراه معتبر نیست.");
        }

        if (row.PezhvakMobile is not null && !IranianMobileRegex.IsMatch(row.PezhvakMobile))
        {
            return RowFailure(row.RowNumber, "فرمت شماره پژواک معتبر نیست.");
        }

        DateOnly? birthDate = null;
        if (row.BirthDateRaw is not null)
        {
            birthDate = JalaliConverter.ParseToGregorian(row.BirthDateRaw);
            if (birthDate is null)
            {
                return RowFailure(row.RowNumber, "تاریخ تولد شمسی معتبر نیست.");
            }
        }

        var yearsText = PersianDigitNormalizer.NormalizeAndTrim(row.ServiceYearsRaw);
        var monthsText = PersianDigitNormalizer.NormalizeAndTrim(row.ServiceMonthsRaw);
        var hasYears = !string.IsNullOrEmpty(yearsText);
        var hasMonths = !string.IsNullOrEmpty(monthsText);

        if (hasYears != hasMonths)
        {
            return RowFailure(row.RowNumber, "سال و ماه سابقه باید با هم وارد شوند.");
        }

        HerasatServiceRecord? serviceRecord = null;
        if (hasYears && hasMonths)
        {
            if (!int.TryParse(yearsText, out var years) || years < 0)
            {
                return RowFailure(row.RowNumber, "سال سابقه باید عدد نامنفی باشد.");
            }

            if (!int.TryParse(monthsText, out var months) || months < 0 || months > 11)
            {
                return RowFailure(row.RowNumber, "ماه سابقه باید بین ۰ تا ۱۱ باشد.");
            }

            serviceRecord = new HerasatServiceRecord(years, months);
        }

        try
        {
            var employee = Employee.Create(
                Guid.NewGuid(),
                tenantId,
                organizationId,
                row.PersonnelCode,
                row.FirstName,
                row.LastName,
                row.NationalCode,
                mobile!,
                null,
                _clock.UtcNow,
                birthDate,
                serviceRecord,
                row.PezhvakMobile);

            return Result<Employee>.Success(employee);
        }
        catch (ArgumentException)
        {
            return RowFailure(row.RowNumber, "مقادیر ردیف با قواعد دامنه‌ای پرسنل سازگار نیست.");
        }
    }

    /// <summary>
    /// ساخت نتیجه ناموفق خطای ردیف پرسنل.
    /// </summary>
    /// <param name="row">شماره ردیف</param>
    /// <param name="detail">توضیح خطا</param>
    /// <returns>نتیجه ناموفق</returns>
    private static Result<Employee> RowFailure(int row, string detail)
    {
        return Result<Employee>.Failure(ImportErrors.EmployeeRowError(row, detail));
    }
}