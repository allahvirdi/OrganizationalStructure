namespace OrganizationalStructure.Application.Import.DTOs;

/// <summary>
/// یک ردیف داده پست خوانده‌شده از فایل اکسل (قبل از اعتبارسنجی دامنه).
/// </summary>
/// <param name="Code">کد پست</param>
/// <param name="Title">عنوان پست</param>
/// <param name="Description">شرح اختیاری</param>
/// <param name="ParentCode">کد پست والد (خالی یعنی ریشه)</param>
public sealed record ImportPostRowDto(
    string Code,
    string Title,
    string? Description,
    string? ParentCode);

/// <summary>
/// نتیجه موفق بارگذاری ساختار سازمانی از فایل.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد</param>
/// <param name="ImportedCount">تعداد پست‌های ایجادشده</param>
public sealed record ImportPostsResultDto(
    Guid OrganizationId,
    int ImportedCount);