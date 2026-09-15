using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Domain.Encryption;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Infrastructure.UnitTests;

/// <summary>
/// تست‌های محافظ PII با AES.
/// </summary>
public sealed class AesPiiProtectorTests
{
    private static AesPiiProtector CreateProtector()
    {
        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return new AesPiiProtector(Options.Create(new PiiEncryptionOptions { Key = key }));
    }

    /// <summary>
    /// رفت‌وبرگشت رمزنگاری در هر دو حالت باید متن اصلی را برگرداند.
    /// </summary>
    [Theory]
    [InlineData(EncryptionType.Deterministic)]
    [InlineData(EncryptionType.Randomized)]
    public void Protect_ThenUnprotect_ShouldReturnOriginal(EncryptionType type)
    {
        var protector = CreateProtector();
        const string plaintext = "0012345678";

        var cipher = protector.Protect(plaintext, type);

        protector.Unprotect(cipher).Should().Be(plaintext);
    }

    /// <summary>
    /// حالت قطعی باید برای متن یکسان خروجی یکسان بدهد (قابل جستجو).
    /// </summary>
    [Fact]
    public void Protect_Deterministic_ShouldProduceSameOutput()
    {
        var protector = CreateProtector();

        var first = protector.Protect("09120000000", EncryptionType.Deterministic);
        var second = protector.Protect("09120000000", EncryptionType.Deterministic);

        first.Should().Be(second);
    }

    /// <summary>
    /// حالت تصادفی باید برای متن یکسان خروجی متفاوت بدهد.
    /// </summary>
    [Fact]
    public void Protect_Randomized_ShouldProduceDifferentOutputs()
    {
        var protector = CreateProtector();

        var first = protector.Protect("علی", EncryptionType.Randomized);
        var second = protector.Protect("علی", EncryptionType.Randomized);

        first.Should().NotBe(second);
    }

    /// <summary>
    /// کلید نامعتبر باید در ساخت با خطا مواجه شود (fail-closed).
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("not-base64!!!")]
    [InlineData("YWJjZA==")]
    public void Ctor_InvalidKey_ShouldThrow(string key)
    {
        var act = () => new AesPiiProtector(Options.Create(new PiiEncryptionOptions { Key = key }));

        act.Should().Throw<InvalidOperationException>();
    }
}