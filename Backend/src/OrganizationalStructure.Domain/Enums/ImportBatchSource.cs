namespace OrganizationalStructure.Domain.Enums;

/// <summary>
/// منشأ ایجاد بارگذاری واسط.
/// </summary>
public enum ImportBatchSource : byte
{
    /// <summary>بارگذاری فایل از سامانه.</summary>
    File = 1,

    /// <summary>درج بیرونی مستقیم در جدول واسط.</summary>
    External = 2
}