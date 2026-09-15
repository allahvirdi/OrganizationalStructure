using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Events;

/// <summary>
/// رویداد دامنه: پرسنل جدید ثبت شد.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="TenantId">شناسه مستأجر</param>
/// <param name="PersonnelCode">کد پرسنلی</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeCreated(
    Guid EmployeeId,
    Guid TenantId,
    string PersonnelCode,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: اطلاعات پرسنلی ویرایش شد.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeUpdated(
    Guid EmployeeId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پرسنل فعال شد.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeActivated(
    Guid EmployeeId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پرسنل غیرفعال شد.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeDeactivated(
    Guid EmployeeId,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: پرسنل به پست منتسب شد.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="IsPrimary">آیا انتساب اصلی است؟</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeAssignedToPost(
    Guid EmployeeId,
    Guid PostId,
    bool IsPrimary,
    DateTimeOffset OccurredOn) : IDomainEvent;

/// <summary>
/// رویداد دامنه: انتساب پرسنل به پست پایان یافت.
/// </summary>
/// <param name="EmployeeId">شناسه پرسنل</param>
/// <param name="PostId">شناسه پست</param>
/// <param name="OccurredOn">زمان وقوع</param>
public sealed record EmployeeAssignmentEnded(
    Guid EmployeeId,
    Guid PostId,
    DateTimeOffset OccurredOn) : IDomainEvent;