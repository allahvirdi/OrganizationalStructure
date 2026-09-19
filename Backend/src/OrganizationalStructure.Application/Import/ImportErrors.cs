using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Import;

/// <summary>
/// کدهای خطای ورود ساختار سازمانی از فایل برای Error Contract یکپارچه.
/// </summary>
public static class ImportErrors
{
    /// <summary>
    /// ساخت خطای «فایل نامعتبر» (خالی، فرمت نادرست، بدون ردیف داده یا خراب).
    /// </summary>
    /// <param name="detail">توضیح تکمیلی علت نامعتبر بودن</param>
    public static Error InvalidFile(string detail) => new(
        "Import.InvalidFile",
        $"فایل بارگذاری‌شده معتبر نیست: {detail}",
        ErrorType.Validation);

    /// <summary>
    /// ساخت خطای «کد تکراری در فایل».
    /// </summary>
    /// <param name="code">کد تکراری</param>
    public static Error DuplicateCodeInFile(string code) => new(
        "Import.DuplicateCodeInFile",
        $"کد پست {code} بیش از یک بار در فایل آمده است.",
        ErrorType.Validation);

    /// <summary>
    /// ساخت خطای «کد والد یافت نشد» (نه در فایل و نه در دیتابیس).
    /// </summary>
    /// <param name="row">شماره ردیف داده (یک‌مبنایی، پس از هدر)</param>
    /// <param name="parentCode">کد والد ناموجود</param>
    public static Error ParentNotFound(int row, string parentCode) => new(
        "Import.ParentNotFound",
        $"ردیف {row}: کد پست والد «{parentCode}» نه در فایل و نه در پست‌های موجود این سازمان یافت نشد.",
        ErrorType.Validation);

    /// <summary>
    /// ساخت خطای «بیش از حد مجاز ردیف».
    /// </summary>
    /// <param name="maxRows">حداکثر تعداد ردیف مجاز</param>
    public static Error RowLimitExceeded(int maxRows) => new(
        "Import.RowLimitExceeded",
        $"تعداد ردیف‌های فایل از حداکثر مجاز ({maxRows}) بیشتر است.",
        ErrorType.Validation);
}