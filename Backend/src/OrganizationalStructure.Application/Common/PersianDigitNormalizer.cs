namespace OrganizationalStructure.Application.Common;

/// <summary>
/// نرمال‌سازی ارقام فارسی/عربی به لاتین برای داده‌های ورودی دست‌نویس (اکسل/CSV).
/// </summary>
/// <remarks>
/// ارقام فارسی: U+06F0..U+06F9 (۰..۹)
/// ارقام عربی: U+0660..U+0669 (٠..٩)
/// همچنین کاراکترهای جداکننده هزارگان (U+00A0، U+200C نیم‌فاصله، ویرگول) حذف می‌شوند.
/// </remarks>
public static class PersianDigitNormalizer
{
    /// <summary>
    /// تبدیل تمام ارقام فارسی/عربی رشته به معادل لاتین و حذف جداکننده‌های رایج.
    /// </summary>
    /// <param name="value">رشته ورودی (ممکن است خالی باشد)</param>
    /// <returns>رشته نرمال‌شده با ارقام لاتین؛ null در صورت ورودی null</returns>
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var buffer = new System.Text.StringBuilder(value.Length);

        foreach (var ch in value)
        {
            switch (ch)
            {
                // ارقام فارسی U+06F0..U+06F9
                case >= '\u06F0' and <= '\u06F9':
                    buffer.Append((char)('0' + (ch - '\u06F0')));
                    break;

                // ارقام عربی U+0660..U+0669
                case >= '\u0660' and <= '\u0669':
                    buffer.Append((char)('0' + (ch - '\u0660')));
                    break;

                // نیم‌فاصله (ZWNJ) — حذف
                case '\u200C':
                case '\u200D':
                    break;

                // فاصله غیرشکننده (nbsp) — حذف
                case '\u00A0':
                    break;

                // ویرگول فارسی (U+060C) و جداکننده هزارگان — حذف
                case '\u060C':
                case ',':
                    break;

                default:
                    buffer.Append(ch);
                    break;
            }
        }

        return buffer.ToString();
    }

    /// <summary>
    /// نرمال‌سازی و حذف فاصله‌های اضافی از ابتدا و انتها.
    /// </summary>
    /// <param name="value">رشته ورودی</param>
    /// <returns>رشته نرمال‌شده و Trim شده؛ null در صورت ورودی null</returns>
    public static string? NormalizeAndTrim(string? value)
    {
        return Normalize(value)?.Trim();
    }
}