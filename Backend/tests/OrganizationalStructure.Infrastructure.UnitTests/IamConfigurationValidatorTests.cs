using FluentAssertions;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Infrastructure.UnitTests;

/// <summary>
/// تست‌های بررسی پیکربندی اتصال به سامانه هویت (IAM).
/// </summary>
public sealed class IamConfigurationValidatorTests
{
    private static IamOptions CompleteOptions() => new()
    {
        BaseAddress = "http://localhost:5000",
        ClientId = "personnel-bff",
        ClientSecret = "dev-secret"
    };

    /// <summary>
    /// پیکربندی کامل نباید هیچ کلید بدون مقداری گزارش کند.
    /// </summary>
    [Fact]
    public void GetMissingSettings_CompleteOptions_ShouldReturnEmpty()
    {
        var missing = IamConfigurationValidator.GetMissingSettings(CompleteOptions());

        missing.Should().BeEmpty();
    }

    /// <summary>
    /// نبود آدرس پایه باید به‌عنوان کلید بدون مقدار گزارش شود.
    /// </summary>
    [Fact]
    public void GetMissingSettings_WithoutBaseAddress_ShouldReportBaseAddress()
    {
        var options = CompleteOptions();
        options.BaseAddress = string.Empty;

        var missing = IamConfigurationValidator.GetMissingSettings(options);

        missing.Should().Equal("Iam:BaseAddress");
    }

    /// <summary>
    /// نبود شناسه کلاینت باید به‌عنوان کلید بدون مقدار گزارش شود.
    /// </summary>
    [Fact]
    public void GetMissingSettings_WithoutClientId_ShouldReportClientId()
    {
        var options = CompleteOptions();
        options.ClientId = string.Empty;

        var missing = IamConfigurationValidator.GetMissingSettings(options);

        missing.Should().Equal("Iam:ClientId");
    }

    /// <summary>
    /// نبود رمز کلاینت باید به‌عنوان کلید بدون مقدار گزارش شود (خطای رایج اتصال ناموفق به IAM).
    /// </summary>
    [Fact]
    public void GetMissingSettings_WithoutClientSecret_ShouldReportClientSecret()
    {
        var options = CompleteOptions();
        options.ClientSecret = string.Empty;

        var missing = IamConfigurationValidator.GetMissingSettings(options);

        missing.Should().Equal("Iam:ClientSecret");
    }

    /// <summary>
    /// مقدارهای فقط-فاصله به‌عنوان «بدون مقدار» در نظر گرفته می‌شوند.
    /// </summary>
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    public void GetMissingSettings_WhitespaceValues_ShouldBeTreatedAsMissing(string whitespace)
    {
        var options = new IamOptions
        {
            BaseAddress = whitespace,
            ClientId = whitespace,
            ClientSecret = whitespace
        };

        var missing = IamConfigurationValidator.GetMissingSettings(options);

        missing.Should().Equal("Iam:BaseAddress", "Iam:ClientId", "Iam:ClientSecret");
    }

    /// <summary>
    /// تنظیمات نال باید استثنا بدهد (fail-fast).
    /// </summary>
    [Fact]
    public void GetMissingSettings_NullOptions_ShouldThrow()
    {
        var act = () => IamConfigurationValidator.GetMissingSettings(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
