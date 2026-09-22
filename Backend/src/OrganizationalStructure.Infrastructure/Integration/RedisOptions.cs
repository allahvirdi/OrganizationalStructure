namespace OrganizationalStructure.Infrastructure.Integration;

/// <summary>
/// تنظیمات اتصال Redis برای نگهداری نشست‌های BFF در استقرار چندنمونه‌ای (Phase 8).
/// </summary>
/// <remarks>
/// Connection string از بخش <c>ConnectionStrings:Redis</c> خوانده می‌شود؛
/// این کلاس فقط تنظیمات تکمیلی را حمل می‌کند.
/// </remarks>
public sealed class RedisOptions
{
    /// <summary>
    /// نام بخش پیکربندی.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// پیشوند کلیدهای Redis برای جداسازی فضای نام سامانه.
    /// </summary>
    public string KeyPrefix { get; set; } = "orgstructure";
}