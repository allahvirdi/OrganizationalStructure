namespace OrganizationalStructure.Application.Import.DTOs;

/// <summary>
/// یک ردیف داده پرسنل خوانده‌شده از فایل اکسل/CSV (قبل از اعتبارسنجی دامنه).
/// </summary>
/// <param name="RowNumber">شماره ردیف داده در فایل (یک‌مبنایی، پس از هدر) — برای گزارش خطا</param>
/// <param name="PersonnelCode">کد پرسنلی (۸ رقمی)</param>
/// <param name="FirstName">نام</param>
/// <param name="LastName">نام خانوادگی</param>
/// <param name="NationalCode">کد ملی (۱۰ رقمی)</param>
/// <param name="Mobile">شماره همراه (اختیاری در فایل؛ فرمت ایرانی)</param>
/// <param name="BirthDateRaw">تاریخ تولد خام از فایل (قالب جلالی yyyy/MM/dd؛ ارقام فارسی مجاز)</param>
/// <param name="ServiceYearsRaw">سال سابقه حراست (رشته؛ ممکن است خالی باشد)</param>
/// <param name="ServiceMonthsRaw">ماه سابقه حراست (رشته؛ ممکن است خالی باشد)</param>
/// <param name="PezhvakMobile">شماره پژواک (اختیاری)</param>
public sealed record ImportEmployeeRowDto(
    int RowNumber,
    string PersonnelCode,
    string FirstName,
    string LastName,
    string NationalCode,
    string? Mobile,
    string? BirthDateRaw,
    string? ServiceYearsRaw,
    string? ServiceMonthsRaw,
    string? PezhvakMobile);

/// <summary>
/// نتیجه موفق بارگذاری دسته‌جمعی پرسنل از فایل.
/// </summary>
/// <param name="OrganizationId">شناسه سازمان مقصد</param>
/// <param name="ImportedCount">تعداد پرسنل ایجادشده</param>
public sealed record ImportEmployeesResultDto(
    Guid OrganizationId,
    int ImportedCount);