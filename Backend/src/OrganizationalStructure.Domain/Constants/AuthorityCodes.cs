namespace OrganizationalStructure.Domain.Constants;

/// <summary>
/// کدهای پیشنهادی اختیار (Proposed — بدون Seed تا تأیید Business Catalog).
/// </summary>
/// <remarks>
/// از ۲۰۲۶-۰۹-۲۳ (DEC-035) کد اختیار/حق امضا به‌صورت خودکار (GUID) تولید می‌شود؛
/// بنابراین نشان «صاحب امضا» بر پایه وجود انتساب جاری محاسبه می‌شود، نه این کد.
/// این ثابت فقط به‌عنوان مرجع/سازگاری با داده تاریخی نگه داشته شده است.
/// </remarks>
public static class AuthorityCodes
{
    /// <summary>
    /// کد پیشنهادی اختیار امضا.
    /// </summary>
    public const string SigningAuthority = "SIGNING_AUTHORITY";
}