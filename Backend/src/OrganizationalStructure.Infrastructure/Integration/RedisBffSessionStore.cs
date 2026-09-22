using System.Text.Json;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;
using StackExchange.Redis;

namespace OrganizationalStructure.Infrastructure.Integration;

/// <summary>
/// مخزن Redis نشست‌های BFF برای استقرار چندنمونه‌ای (جایگزین <see cref="InMemoryBffSessionStore"/> در Production).
/// </summary>
/// <remarks>
/// انقضای نشست به‌صورت TTL روی کلید تنظیم می‌شود؛ پس از انقضا، Redis کلید را خودکار حذف می‌کند.
/// ساختار <see cref="BffSession"/> با <see cref="System.Text.Json"/> سریال می‌شود.
/// </remarks>
public sealed class RedisBffSessionStore : IBffSessionStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IClock _clock;
    private readonly RedisOptions _options;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="redis">اتصال Redis (Singleton)</param>
    /// <param name="clock">ساعت سامانه برای محاسبه TTL</param>
    /// <param name="options">تنظیمات پیشوند کلید</param>
    public RedisBffSessionStore(
        IConnectionMultiplexer redis,
        IClock clock,
        IOptions<RedisOptions> options)
    {
        _redis = redis;
        _clock = clock;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task SaveAsync(BffSession session, CancellationToken cancellationToken = default)
    {
        var ttl = session.ExpiresAt - _clock.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(session, JsonOptions);
        await db.StringSetAsync(BuildKey(session.SessionId), json, ttl);
    }

    /// <inheritdoc />
    public async Task<BffSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(BuildKey(sessionId));
        if (!value.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<BffSession>(value.ToString(), JsonOptions);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(BuildKey(sessionId));
    }

    /// <summary>
    /// ساخت کلید یکتای نشست با پیشوند پیکربندی‌شده.
    /// </summary>
    private string BuildKey(string sessionId) => $"{_options.KeyPrefix}:bff-session:{sessionId}";
}