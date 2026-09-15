using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Events;

/// <summary>
/// رویداد دامنه: مسئولیت سازمانی تعریف شد.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="TenantId">شناسه مستأجر</param>
/// <param name="Code">کد مسئولیت</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record ResponsibilityCreated(
    Guid ResponsibilityId,
    Guid TenantId,
    string Code,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: مسئولیت سازمانی ویرایش شد.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record ResponsibilityUpdated(
    Guid ResponsibilityId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: مسئولیت سازمانی غیرفعال شد.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record ResponsibilityDeactivated(
    Guid ResponsibilityId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: مسئولیت به پست منتسب شد.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="OrganizationId">شناسه سازمان (Scope انتساب)</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record ResponsibilityAssigned(
    Guid ResponsibilityId,
    Guid PostId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: انتساب مسئولیت به پست پایان یافت.
/// </summary>
/// <param name="ResponsibilityId">شناسه مسئولیت</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record ResponsibilityAssignmentEnded(
    Guid ResponsibilityId,
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: اختیار سازمانی تعریف شد.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="TenantId">شناسه مستأجر</param>
/// <param name="Code">کد اختیار</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record AuthorityCreated(
    Guid AuthorityId,
    Guid TenantId,
    string Code,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: اختیار سازمانی ویرایش شد.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record AuthorityUpdated(
    Guid AuthorityId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: اختیار سازمانی غیرفعال شد.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record AuthorityDeactivated(
    Guid AuthorityId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: اختیار به پست منتسب شد.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="OrganizationId">شناسه سازمان (Scope انتساب)</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record AuthorityAssigned(
    Guid AuthorityId,
    Guid PostId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: انتساب اختیار به پست پایان یافت.
/// </summary>
/// <param name="AuthorityId">شناسه اختیار</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record AuthorityAssignmentEnded(
    Guid AuthorityId,
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;