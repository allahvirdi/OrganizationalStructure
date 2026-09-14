namespace OrganizationalStructure.Application.Common;

/// <summary>
/// نوع خطای کسب‌وکاری/فنی برای Error Contract یکپارچه.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// درخواست معتبر نیست (Validation).
    /// </summary>
    Validation = 0,

    /// <summary>
    /// موجودیت پیدا نشد.
    /// </summary>
    NotFound = 1,

    /// <summary>
    /// کاربر مجوز انجام عملیات را ندارد (Deny by Default).
    /// </summary>
    Forbidden = 2,

    /// <summary>
    /// احراز هویت انجام نشده است.
    /// </summary>
    Unauthorized = 3,

    /// <summary>
    /// برخورد با وضعیت ناسازگار دامنه (Business Rule Violation).
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// خطای داخلی/پیش‌بینی‌نشده سمت سرور.
    /// </summary>
    Internal = 5
}

/// <summary>
/// نمایش ساخت‌یافته یک خطا.
/// </summary>
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type)
{
    /// <summary>
    /// بررسی اینکه آیا خطا از نوع اعتبارسنجی است.
    /// </summary>
    public bool IsValidation => Type == ErrorType.Validation;
}