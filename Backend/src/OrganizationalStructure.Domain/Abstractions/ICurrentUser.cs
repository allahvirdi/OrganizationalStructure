namespace OrganizationalStructure.Domain.Abstractions;

/// <summary>
/// دسترسی به اطلاعات کاربر جاری از token/Claimهای احراز هویت.
/// </summary>
/// <remarks>
/// هویت کاربر از IAM تأمین می‌شود؛ این واسط فقط اطلاعات احرازشده‌ی کاربر جاری را در اختیار
/// لایه‌های دامنه و کاربرد می‌گذارد. Relation سازمانی به‌تنهایی مجوز نیست (ADR-008).
/// </remarks>
public interface ICurrentUser
{
    /// <summary>
    /// آیا کاربر احراز هویت شده است؟
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// شناسه کاربر جاری (از IAM).
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// شناسه مستأجر جاری.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// شناسه سازمان جاری کاربر (در صورت وجود؛ از Claim).
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// نقش‌های کاربر جاری.
    /// </summary>
    IReadOnlyCollection<string> Roles { get; }
}