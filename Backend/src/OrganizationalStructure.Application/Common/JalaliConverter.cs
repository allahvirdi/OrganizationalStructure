using System.Globalization;

namespace OrganizationalStructure.Application.Common;

/// <summary>
/// تبدیل رشته تاریخ جلالی (شمسی) به تاریخ میلادی برای داده‌های ورودی فایل.
/// </summary>
/// <remarks>
/// از <see cref="PersianCalendar"/> استاندارد .NET استفاده می‌کند و ارقام فارسی/عربی را
/// پیش از تبدیل نرمال‌سازی می‌نماید. خروجی همیشه <see cref="DateOnly"/> میلادی است.
/// </remarks>
public static class JalaliConverter
{
    /// <summary>
    /// تقویم جلالی استاندارد .NET.
    /// </summary>
    private static readonly PersianCalendar Calendar = new();

    /// <summary>
    /// تجزیه رشته تاریخ شمسی به تاریخ میلادی.
    /// </summary>
    /// <param name="value">رشته به قالب yyyy/MM/dd، yyyy-MM-dd یا yyyy.M.d (ارقام فارسی مجاز)</param>
    /// <returns>تاریخ میلادی در صورت معتبر بودن؛ در غیر این صورت null</returns>
    public static DateOnly? ParseToGregorian(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = PersianDigitNormalizer.NormalizeAndTrim(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        var parts = normalized.Split(['/', '-', '.'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            return null;
        }

        if (!int.TryParse(parts[0], out var year) ||
            !int.TryParse(parts[1], out var month) ||
            !int.TryParse(parts[2], out var day))
        {
            return null;
        }

        try
        {
            var dateTime = Calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return DateOnly.FromDateTime(dateTime);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
        catch (OverflowException)
        {
            return null;
        }
    }

    /// <summary>
    /// بررسی اعتبار تاریخ شمسی.
    /// </summary>
    /// <param name="year">سال جلالی</param>
    /// <param name="month">ماه جلالی</param>
    /// <param name="day">روز جلالی</param>
    /// <returns>در صورت معتبر بودن true</returns>
    public static bool IsValidDate(int year, int month, int day)
    {
        try
        {
            Calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
    }
}