using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Events;

/// <summary>
/// رویداد دامنه: پست سازمانی ایجاد شد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="TenantId">شناسه مستأجر</param>
/// <param name="OrganizationId">شناسه سازمان (مرجع IAM)</param>
/// <param name="Code">کد پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostCreated(
    Guid PostId,
    Guid TenantId,
    Guid OrganizationId,
    string Code,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پست سازمانی ویرایش شد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostUpdated(
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: جایگاه پست در درخت تغییر کرد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OldParentId">شناسه والد قبلی (خالی یعنی ریشه)</param>
/// <param name="NewParentId">شناسه والد جدید (خالی یعنی ریشه)</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostMoved(
    Guid PostId,
    Guid? OldParentId,
    Guid? NewParentId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پست سازمانی فعال شد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostActivated(
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پست سازمانی غیرفعال شد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostDeactivated(
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: وضعیت صاحب‌امضا بودن پست تغییر کرد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="HasSigningAuthority">وضعیت جدید صاحب‌امضا بودن</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record SigningAuthorityChanged(
    Guid PostId,
    bool HasSigningAuthority,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: مجموعه مسئولیت‌های پست تغییر کرد.
/// </summary>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record PostResponsibilitiesChanged(
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;