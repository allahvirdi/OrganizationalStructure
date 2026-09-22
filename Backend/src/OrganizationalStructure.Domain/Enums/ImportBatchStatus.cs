namespace OrganizationalStructure.Domain.Enums;

/// <summary>
/// وضعیت چرخه حیات یک بارگذاری واسط.
/// </summary>
public enum ImportBatchStatus : byte
{
    /// <summary>در انتظار درج ردیف‌ها (فقط مسیر بیرونی).</summary>
    AwaitingRows = 1,

    /// <summary>ردیف‌ها درج و اعتبارسنجی شده؛ آماده بازبینی.</summary>
    Ready = 2,

    /// <summary>تأیید و ثبت نهایی انجام شده.</summary>
    Committed = 3,

    /// <summary>بارگذاری رد شده.</summary>
    Rejected = 4
}