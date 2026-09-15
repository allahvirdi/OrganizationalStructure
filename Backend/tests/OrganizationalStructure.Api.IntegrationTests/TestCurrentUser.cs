using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// کاربر جاری ثابت برای تست‌های یکپارچگی (شبیه‌سازی مستأجر احرازشده).
/// </summary>
public sealed class TestCurrentUser : ICurrentUser
{
    /// <summary>
    /// شناسه مستأجر ثابت تست.
    /// </summary>
    public static readonly Guid TestTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    /// <inheritdoc />
    public bool IsAuthenticated => true;

    /// <inheritdoc />
    public Guid UserId { get; } = Guid.Parse("22222222-2222-2222-2222-222222222222");

    /// <inheritdoc />
    public Guid TenantId => TestTenantId;

    /// <inheritdoc />
    public Guid? OrganizationId => null;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles => Array.Empty<string>();
}