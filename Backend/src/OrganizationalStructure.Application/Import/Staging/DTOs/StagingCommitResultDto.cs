namespace OrganizationalStructure.Application.Import.Staging.DTOs;

/// <summary>
/// نتیجه ثبت نهایی بارگذاری واسط.
/// </summary>
/// <param name="BatchId">شناسه بارگذاری</param>
/// <param name="CommittedCount">تعداد ردیف‌های ثبت‌شده در جدول پرسنل</param>
/// <param name="SkippedCount">تعداد ردیف‌های نامعتبر/تکراری که ثبت نشدند</param>
/// <param name="Errors">فهرست خطاهای ردیف‌های ردشده (شماره ردیف + علت)</param>
public sealed record StagingCommitResultDto(
    Guid BatchId,
    int CommittedCount,
    int SkippedCount,
    IReadOnlyList<StagingCommitErrorDto> Errors);

/// <summary>
/// خطای یک ردیف در هنگام ثبت نهایی.
/// </summary>
/// <param name="RowNumber">شماره ردیف در فایل / ترتیب درج</param>
/// <param name="Message">پیام خطا — بدون افشای مقدار حساس (ADR-006)</param>
public sealed record StagingCommitErrorDto(
    int RowNumber,
    string Message);