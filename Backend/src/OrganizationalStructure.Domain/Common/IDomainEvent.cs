namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// رابط پایه رویدادهای دامنه.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// زمان وقوع رویداد (به‌صورت UTC).
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}