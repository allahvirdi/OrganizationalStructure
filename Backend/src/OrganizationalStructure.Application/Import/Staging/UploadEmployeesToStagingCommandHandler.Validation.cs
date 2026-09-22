using OrganizationalStructure.Application.Common;
using OrganizationalStructure.Application.Import.DTOs;
using System.Text.RegularExpressions;

namespace OrganizationalStructure.Application.Import.Staging;

/// <summary>
/// قواعد اعتبارسنجی ردیف در پردازش‌گر بارگذاری واسط.
/// </summary>
public sealed partial class UploadEmployeesToStagingCommandHandler
{
    /// <summary>الگوی کد پرسنلی ۸ رقمی.</summary>
    private static readonly Regex PersonnelCodeRegex = new("^[0-9]{8}$", RegexOptions.Compiled);

    /// <summary>الگوی موبایل ایرانی.</summary>
    private static readonly Regex IranianMobileRegex = new("^09[0-9]{9}$", RegexOptions.Compiled);

    /// <summary>
    /// اعتبارسنجی یک ردیف و بازگشت فهرست خطاها (خالی = معتبر).
    /// </summary>
    private static List<string> ValidateRow(
        ImportEmployeeRowDto row,
        HashSet<string> seenCodes,
        HashSet<string> seenNationalCodes)
    {
        var errors = new List<string>();

        if (!PersonnelCodeRegex.IsMatch(row.PersonnelCode))
        {
            errors.Add("کد پرسنلی باید دقیقاً ۸ رقم باشد.");
        }
        else if (!seenCodes.Add(row.PersonnelCode))
        {
            errors.Add("کد پرسنلی تکراری در فایل.");
        }

        if (!IranianNationalCodeValidator.IsValid(row.NationalCode))
        {
            errors.Add("کد ملی معتبر نیست.");
        }
        else if (!seenNationalCodes.Add(row.NationalCode))
        {
            errors.Add("کد ملی تکراری در فایل.");
        }

        if (row.Mobile is not null && !IranianMobileRegex.IsMatch(row.Mobile))
        {
            errors.Add("فرمت شماره همراه معتبر نیست.");
        }

        if (row.BirthDateRaw is not null && ParseBirthDate(row.BirthDateRaw) is null)
        {
            errors.Add("تاریخ تولد شمسی معتبر نیست.");
        }

        var yearsText = PersianDigitNormalizer.NormalizeAndTrim(row.ServiceYearsRaw);
        var monthsText = PersianDigitNormalizer.NormalizeAndTrim(row.ServiceMonthsRaw);
        var hasYears = !string.IsNullOrEmpty(yearsText);
        var hasMonths = !string.IsNullOrEmpty(monthsText);

        if (hasYears != hasMonths)
        {
            errors.Add("سال و ماه سابقه باید با هم وارد شوند.");
        }
        else if (hasYears)
        {
            if (!int.TryParse(yearsText, out var years) || years < 0 || years > 50)
            {
                errors.Add("سال سابقه باید عدد بین ۰ تا ۵۰ باشد.");
            }

            if (!int.TryParse(monthsText, out var months) || months < 0 || months > 11)
            {
                errors.Add("ماه سابقه باید بین ۰ تا ۱۱ باشد.");
            }
        }

        return errors;
    }

    /// <summary>تبدیل تاریخ شمسی خام به میلادی (یا null).</summary>
    private static DateOnly? ParseBirthDate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return JalaliConverter.ParseToGregorian(raw);
    }

    /// <summary>تبدیل رشته عددی به int (یا null).</summary>
    private static int? ParseInt(string? raw)
    {
        var normalized = PersianDigitNormalizer.NormalizeAndTrim(raw);
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        return int.TryParse(normalized, out var value) ? value : null;
    }
}