namespace OrganizationalStructure.Domain.Encryption;

/// <summary>
/// مشخص می‌کند که ویژگی مربوطه داده حساس (PII) است و هنگام ذخیره‌سازی باید رمزنگاری شود.
/// </summary>
/// <remarks>
/// اعمال واقعی رمزنگاری در لایه زیرساخت (پیکربندی EF Core) انجام می‌شود؛
/// این Attribute فقط اعلام سیاست در سطح دامنه است (ADR-006).
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PiiEncryptedAttribute : Attribute
{
    /// <summary>
    /// نوع رمزنگاری موردنیاز.
    /// </summary>
    public EncryptionType EncryptionType { get; }

    /// <summary>
    /// ساخت Attribute.
    /// </summary>
    /// <param name="encryptionType">نوع رمزنگاری</param>
    public PiiEncryptedAttribute(EncryptionType encryptionType)
    {
        EncryptionType = encryptionType;
    }
}