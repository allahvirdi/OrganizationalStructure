using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Infrastructure.Common;

/// <summary>
/// پیاده‌سازی واقعی ساعت مبتنی بر ساعت سیستم.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}