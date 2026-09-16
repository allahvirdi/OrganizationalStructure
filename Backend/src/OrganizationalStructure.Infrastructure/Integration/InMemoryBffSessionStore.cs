using System.Collections.Concurrent;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Infrastructure.Integration;

/// <summary>
/// مخزن درون‌حافظه‌ای نشست‌های BFF (تک‌نمونه؛ جایگزینی با Redis برای چندنمونه‌ای در Phase 8).
/// </summary>
public sealed class InMemoryBffSessionStore : IBffSessionStore
{
    private readonly ConcurrentDictionary<string, BffSession> _sessions = new();
    private readonly IClock _clock;

    /// <summary>
    /// مقداردهی اولیه.
    /// </summary>
    /// <param name="clock">ساعت</param>
    public InMemoryBffSessionStore(IClock clock)
    {
        _clock = clock;
    }

    /// <inheritdoc />
    public Task SaveAsync(BffSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.SessionId] = session;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<BffSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return Task.FromResult<BffSession?>(null);
        }

        if (session.ExpiresAt <= _clock.UtcNow)
        {
            _sessions.TryRemove(sessionId, out _);
            return Task.FromResult<BffSession?>(null);
        }

        return Task.FromResult<BffSession?>(session);
    }

    /// <inheritdoc />
    public Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        _sessions.TryRemove(sessionId, out _);
        return Task.CompletedTask;
    }
}