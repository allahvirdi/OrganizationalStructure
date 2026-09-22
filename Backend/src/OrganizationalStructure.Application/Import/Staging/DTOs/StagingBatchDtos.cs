using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Application.Import.Staging.DTOs;

/// <summary>
/// DTO بارگذاری واسط (فهرست بارگذاری‌ها).
/// </summary>
public sealed record ImportBatchDto
{
    /// <summary>
    /// شناسه بارگذاری.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// شناسه سازمان مالک.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// نام سازمان (از Scope کاربر؛ ممکن است خالی باشد).
    /// </summary>
    public string? OrganizationName { get; init; }

    /// <summary>
    /// منشأ بارگذاری.
    /// </summary>
    public ImportBatchSource Source { get; init; }

    /// <summary>
    /// نام فایل اصلی (فقط مسیر فایل).
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// وضعیت چرخه حیات.
    /// </summary>
    public ImportBatchStatus Status { get; init; }

    /// <summary>
    /// تعداد کل ردیف‌ها.
    /// </summary>
    public int TotalRows { get; init; }

    /// <summary>
    /// تعداد ردیف‌های معتبر.
    /// </summary>
    public int ValidRows { get; init; }

    /// <summary>
    /// تعداد ردیف‌های نامعتبر.
    /// </summary>
    public int InvalidRows { get; init; }

    /// <summary>
    /// تعداد ثبت‌شده نهایی در لحظه Commit.
    /// </summary>
    public int? CommittedCount { get; init; }

    /// <summary>
    /// یادداشت بازبین.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// زمان ایجاد بارگذاری.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// زمان تأیید/رد.
    /// </summary>
    public DateTimeOffset? ReviewedAt { get; init; }

    /// <summary>
    /// زمان ثبت نهایی.
    /// </summary>
    public DateTimeOffset? CommittedAt { get; init; }
}

/// <summary>
/// DTO ردیف واسط پرسنل همراه خطاهای اعتبارسنجی.
/// </summary>
public sealed record StagingRowDto
{
    /// <summary>
    /// شناسه ردیف.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// شماره ردیف در فایل / ترتیب درج.
    /// </summary>
    public int RowNumber { get; init; }

    /// <summary>
    /// کد پرسنلی.
    /// </summary>
    public string PersonnelCode { get; init; } = string.Empty;

    /// <summary>
    /// نام.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// نام خانوادگی.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// کد ملی.
    /// </summary>
    public string NationalCode { get; init; } = string.Empty;

    /// <summary>
    /// موبایل.
    /// </summary>
    public string? Mobile { get; init; }

    /// <summary>
    /// تاریخ تولد.
    /// </summary>
    public DateOnly? BirthDate { get; init; }

    /// <summary>
    /// سال‌های سابقه خدمت.
    /// </summary>
    public int? ServiceYears { get; init; }

    /// <summary>
    /// ماه‌های سابقه خدمت.
    /// </summary>
    public int? ServiceMonths { get; init; }

    /// <summary>
    /// وضعیت اعتبارسنجی ردیف.
    /// </summary>
    public StagingRowValidationStatus ValidationStatus { get; init; }

    /// <summary>
    /// وضعیت ثبت نهایی.
    /// </summary>
    public StagingRowCommitStatus CommitStatus { get; init; }

    /// <summary>
    /// شناسه Employee ایجادشده پس از Commit.
    /// </summary>
    public Guid? EmployeeId { get; init; }

    /// <summary>
    /// خطاهای ردیف (بدون افشای مقدار حساس — ADR-006).
    /// </summary>
    public IReadOnlyList<StagingRowErrorDto> Errors { get; init; } = Array.Empty<StagingRowErrorDto>();
}

/// <summary>
/// DTO خطای ردیف واسط.
/// </summary>
public sealed record StagingRowErrorDto
{
    /// <summary>
    /// شماره ردیف.
    /// </summary>
    public int? RowNumber { get; init; }

    /// <summary>
    /// نام ستون مرتبط.
    /// </summary>
    public string? ColumnName { get; init; }

    /// <summary>
    /// کد خطا.
    /// </summary>
    public string ErrorCode { get; init; } = string.Empty;

    /// <summary>
    /// پیام خطا — بدون افشای مقدار حساس.
    /// </summary>
    public string Message { get; init; } = string.Empty;
}