using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// ثابت‌های مشترک تست‌های یکپارچگی (فقط تست).
/// </summary>
public static class TestKeys
{
    /// <summary>
    /// کلید PII تست‌-only (با کارخانه هماهنگ است؛ هرگز در Production نیست).
    /// </summary>
    public const string Pii = "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8=";
}

/// <summary>
/// متن مستأجر ثابت برای seed مستقیم (فقط تست).
/// </summary>
public sealed class FixedTenantContext : ITenantContext
{
    /// <summary>
    /// ساخت متن مستأجر ثابت.
    /// </summary>
    public FixedTenantContext(Guid tenantId)
    {
        TenantId = tenantId;
    }

    /// <inheritdoc />
    public Guid TenantId { get; }
}