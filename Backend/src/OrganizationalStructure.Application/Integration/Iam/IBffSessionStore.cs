using OrganizationalStructure.Domain.Abstractions;

namespace OrganizationalStructure.Application.Integration.Iam;

/// <summary>
/// نشست سمت‌سرور BFF (توکن‌های IAM فقط اینجا نگهداری می‌شوند، هرگز در مرورگر).
/// </summary>
public sealed record BffSession
{
    /// <summary>
    /// شناسه نشست BFF (مقدار کوکی HttpOnly).
    /// </summary>
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// توکن دسترسی IAM.
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// توکن بازآوری IAM.
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// زمان انقضای نشست.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// زمان آخرین اعتبارسنجی موفق.
    /// </summary>
    public DateTimeOffset ValidatedAt { get; init; }

    /// <summary>
    /// شناسه کاربر.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// شناسه مستأجر.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// شناسه سازمان.
    /// </summary>
    public string? OrganizationId { get; init; }

    /// <summary>
    /// نقش‌ها.
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// دسترسی‌ها.
    /// </summary>
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// شناسه‌های سازمان‌های داخل Scope مشاهده (خود سازمان + زیرمجموعه‌ها).
    /// </summary>
    public IReadOnlyList<Guid> VisibleOrganizationIds { get; init; } = Array.Empty<Guid>();

    /// <summary>
    /// سازمان‌های داخل Scope مشاهده همراه با نام و کد (برای نمایش/انتخاب سازمان در UI).
    /// </summary>
    public IReadOnlyList<OrganizationReference> VisibleOrganizations { get; init; } =
        Array.Empty<OrganizationReference>();
}

/// <summary>
/// مخزن سمت‌سرور نشست‌های BFF.
/// </summary>
public interface IBffSessionStore
{
    /// <summary>
    /// ذخیره نشست.
    /// </summary>
    Task SaveAsync(BffSession session, CancellationToken cancellationToken = default);

    /// <summary>
    /// بازیابی نشست با شناسه.
    /// </summary>
    /// <returns>نشست یا خالی در صورت نبود/انقضا</returns>
    Task<BffSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف نشست (خروج).
    /// </summary>
    Task RemoveAsync(string sessionId, CancellationToken cancellationToken = default);
}