namespace OrganizationalStructure.Domain.Encryption;

/// <summary>
/// نوع رمزنگاری ستون برای داده‌های حساس (PII).
/// </summary>
public enum EncryptionType
{
    /// <summary>
    /// قطعی — برای فیلدهای جستجوپذیر و یکتا (کد ملی، موبایل).
    /// </summary>
    Deterministic = 0,

    /// <summary>
    /// تصادفی — برای فیلدهای متنی آزاد (نام، نام خانوادگی).
    /// </summary>
    Randomized = 1
}