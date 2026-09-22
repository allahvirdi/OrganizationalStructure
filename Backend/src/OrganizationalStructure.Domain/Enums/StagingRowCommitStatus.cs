namespace OrganizationalStructure.Domain.Enums;

/// <summary>
/// وضعیت ثبت نهایی یک ردیف واسط.
/// </summary>
public enum StagingRowCommitStatus : byte
{
    /// <summary>در انتظار ثبت.</summary>
    Pending = 1,

    /// <summary>ثبت‌شده در جدول پرسنل.</summary>
    Committed = 2,

    /// <summary>ردشده/نامعتبر — ثبت نشده.</summary>
    Skipped = 3
}