namespace OrganizationalStructure.Infrastructure.Security;

/// <summary>
/// بررسی کامل بودن پیکربندی اتصال به سامانه هویت (IAM).
/// </summary>
/// <remarks>
/// این بررسی خالص و بدون اثر جانبی است و هدف آن جلوگیری از «شکست خاموش» اتصال IAM است؛
/// پیکربندی ناقص باعث می‌شود تمام درخواست‌های ورود به‌صورت fail-closed با خطا رد شوند
/// بدون آن‌که علت واقعی در لاگ مشخص باشد.
/// </remarks>
public static class IamConfigurationValidator
{
    /// <summary>
    /// برگرداندن کلیدهای پیکربندی الزامی که مقدار ندارند.
    /// </summary>
    /// <param name="options">تنظیمات اتصال به سامانه هویت</param>
    /// <returns>فهرست کلیدهای بدون مقدار؛ فهرست خالی یعنی پیکربندی کامل است</returns>
    /// <exception cref="ArgumentNullException">در صورت نال بودن تنظیمات</exception>
    public static IReadOnlyList<string> GetMissingSettings(IamOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BaseAddress))
        {
            missing.Add(Key(nameof(IamOptions.BaseAddress)));
        }

        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            missing.Add(Key(nameof(IamOptions.ClientId)));
        }

        if (string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            missing.Add(Key(nameof(IamOptions.ClientSecret)));
        }

        return missing;
    }

    /// <summary>
    /// ساخت نام کامل کلید پیکربندی.
    /// </summary>
    /// <param name="propertyName">نام خصوصیت تنظیمات</param>
    /// <returns>نام کامل کلید به شکل <c>Iam:Property</c></returns>
    private static string Key(string propertyName) =>
        $"{IamOptions.SectionName}:{propertyName}";
}
