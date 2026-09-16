namespace OrganizationalStructure.Infrastructure.Security;

/// <summary>
/// تنظیمات اتصال سروربه‌سرور به سامانه هویت (IAM).
/// </summary>
/// <remarks>
/// همه مسیرها قابل تنظیم‌اند؛ مقادیر پیش‌فرض از یافته‌های تأییدشده IAM آمده و بدون تأیید حدس نیستند.
/// هیچ رمزی در مخزن ثبت نمی‌شود (Secret Management / متغیر محیطی).
/// </remarks>
public sealed class IamOptions
{
    /// <summary>
    /// نام بخش پیکربندی.
    /// </summary>
    public const string SectionName = "Iam";

    /// <summary>
    /// آدرس پایه سامانه هویت (مثلاً https://iam.example.com).
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;

    /// <summary>
    /// شناسه کلاینت سروربه‌سرور برای Endpointهای توکن (هدر X-Client-Id).
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// رمز کلاینت سروربه‌سرور (هدر X-Client-Secret) — فقط از Secret Management.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// مسیر ورود تعاملی.
    /// </summary>
    public string LoginPath { get; set; } = "/api/auth/login";

    /// <summary>
    /// مسیر تکمیل ورود دومرحله‌ای.
    /// </summary>
    public string VerifyMfaPath { get; set; } = "/api/auth/login/verify-mfa";

    /// <summary>
    /// مسیر اعتبارسنجی توکن.
    /// </summary>
    public string ValidatePath { get; set; } = "/api/token/validate";

    /// <summary>
    /// مسیر تجدید توکن.
    /// </summary>
    public string RefreshPath { get; set; } = "/api/token/refresh";

    /// <summary>
    /// مسیر ابطال توکن.
    /// </summary>
    public string RevokePath { get; set; } = "/api/token/revoke";

    /// <summary>
    /// مهلت فراخوانی‌ها به ثانیه.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// مدت کش اعتبارسنجی موفق به ثانیه (کوتاه؛ fail-closed در انقضا).
    /// </summary>
    public int ValidationCacheSeconds { get; set; } = 60;

    /// <summary>
    /// مدت نشست BFF به دقیقه.
    /// </summary>
    public int SessionMinutes { get; set; } = 30;
}