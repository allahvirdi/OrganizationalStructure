namespace OrganizationalStructure.Application.Import.Staging.DTOs;

/// <summary>
/// نتیجه بارگذاری فایل در جدول واسط.
/// </summary>
/// <param name="BatchId">شناسه بارگذاری ایجادشده</param>
/// <param name="TotalRows">تعداد کل ردیف‌ها</param>
/// <param name="ValidRows">تعداد ردیف‌های معتبر</param>
/// <param name="InvalidRows">تعداد ردیف‌های نامعتبر</param>
public sealed record StagingUploadResultDto(
    Guid BatchId,
    int TotalRows,
    int ValidRows,
    int InvalidRows);