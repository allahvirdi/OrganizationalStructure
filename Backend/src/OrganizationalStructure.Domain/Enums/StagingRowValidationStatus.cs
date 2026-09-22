namespace OrganizationalStructure.Domain.Enums;

/// <summary>
/// وضعیت اعتبارسنجی یک ردیف واسط.
/// </summary>
public enum StagingRowValidationStatus : byte
{
    /// <summary>هنوز اعتبارسنجی نشده.</summary>
    Pending = 1,

    /// <summary>معتبر.</summary>
    Valid = 2,

    /// <summary>نامعتبر.</summary>
    Invalid = 3
}