using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Encryption;

namespace OrganizationalStructure.Infrastructure.Security;

/// <summary>
/// محافظ PII با AES-256-CBC.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>قطعی (Deterministic): بردار اولیه از HMAC متن مشتق می‌شود تا متن یکسان همیشه رمز یکسان بدهد (قابل جستجو با تساوی).</item>
/// <item>تصادفی (Randomized): بردار اولیه تصادفی است و هر بار خروجی متفاوت می‌شود.</item>
/// </list>
/// کلید فقط از پیکربندی امن خوانده می‌شود؛ در نبود کلید معتبر، ساخت نمونه با خطا مواجه می‌شود (fail-closed).
/// </remarks>
public sealed class AesPiiProtector : IPiiProtector
{
    private const int KeySizeBytes = 32;
    private const int IvSizeBytes = 16;

    private readonly byte[] _key;

    /// <summary>
    /// مقداردهی اولیه با کلید پیکربندی.
    /// </summary>
    /// <param name="options">تنظیمات رمزنگاری</param>
    /// <exception cref="InvalidOperationException">در صورت نامعتبر بودن کلید</exception>
    public AesPiiProtector(IOptions<PiiEncryptionOptions> options)
    {
        var keyText = options.Value.Key;
        if (string.IsNullOrWhiteSpace(keyText))
        {
            throw new InvalidOperationException(
                "کلید رمزنگاری PII پیکربندی نشده است (PiiEncryption:Key).");
        }

        byte[] key;
        try
        {
            key = Convert.FromBase64String(keyText);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "کلید رمزنگاری PII باید Base64 معتبر باشد.", ex);
        }

        if (key.Length != KeySizeBytes)
        {
            throw new InvalidOperationException(
                "کلید رمزنگاری PII باید ۲۵۶ بیتی (۳۲ بایت) باشد.");
        }

        _key = key;
    }

    /// <inheritdoc />
    public string Protect(string plaintext, EncryptionType type)
    {
        if (plaintext is null)
        {
            throw new ArgumentNullException(nameof(plaintext));
        }

        var iv = type == EncryptionType.Deterministic
            ? DeriveIv(plaintext)
            : RandomNumberGenerator.GetBytes(IvSizeBytes);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherBytes;
        using (var encryptor = aes.CreateEncryptor())
        {
            cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        var combined = new byte[IvSizeBytes + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, combined, 0, IvSizeBytes);
        Buffer.BlockCopy(cipherBytes, 0, combined, IvSizeBytes, cipherBytes.Length);

        return Convert.ToBase64String(combined);
    }

    /// <inheritdoc />
    public string Unprotect(string ciphertext)
    {
        if (string.IsNullOrWhiteSpace(ciphertext))
        {
            throw new ArgumentException("متن رمزشده معتبر نیست.", nameof(ciphertext));
        }

        var combined = Convert.FromBase64String(ciphertext);
        if (combined.Length <= IvSizeBytes)
        {
            throw new ArgumentException("متن رمزشده معتبر نیست.", nameof(ciphertext));
        }

        var iv = new byte[IvSizeBytes];
        var cipherBytes = new byte[combined.Length - IvSizeBytes];
        Buffer.BlockCopy(combined, 0, iv, 0, IvSizeBytes);
        Buffer.BlockCopy(combined, IvSizeBytes, cipherBytes, 0, cipherBytes.Length);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    private byte[] DeriveIv(string plaintext)
    {
        using var hmac = new HMACSHA256(_key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
        return hash[..IvSizeBytes];
    }
}