using OrganizationalStructure.Domain.Encryption;

namespace OrganizationalStructure.Domain.Abstractions;

/// <summary>
/// قرارداد محافظت از داده‌های حساس (PII) با رمزنگاری.
/// </summary>
public interface IPiiProtector
{
    /// <summary>
    /// رمزنگاری متن شفاف.
    /// </summary>
    /// <param name="plaintext">متن شفاف</param>
    /// <param name="type">نوع رمزنگاری (قطعی برای جستجو، تصادفی برای بقیه)</param>
    /// <returns>متن رمزشده (Base64)</returns>
    string Protect(string plaintext, EncryptionType type);

    /// <summary>
    /// رمزگشایی متن رمزشده.
    /// </summary>
    /// <param name="ciphertext">متن رمزشده (Base64)</param>
    /// <returns>متن شفاف</returns>
    string Unprotect(string ciphertext);
}