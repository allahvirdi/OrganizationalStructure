namespace OrganizationalStructure.Infrastructure.Security;

/// <summary>
/// تنظیمات رمزنگاری PII (کلید از پیکربندی امن — هرگز در مخزن).
/// </summary>
public sealed class PiiEncryptionOptions
{
    /// <summary>
    /// نام بخش پیکربندی.
    /// </summary>
    public const string SectionName = "PiiEncryption";

    /// <summary>
    /// کلید ۲۵۶ بیتی به‌صورت Base64 (۳۲ بایت).
    /// </summary>
    public string Key { get; set; } = string.Empty;
}