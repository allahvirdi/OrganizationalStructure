namespace OrganizationalStructure.Domain.Enums;

/// <summary>
/// وضعیت چرخه حیات موجودیت در سامانه.
/// </summary>
public enum EntityStatus
{
    /// <summary>
    /// فعال.
    /// </summary>
    Active = 0,

    /// <summary>
    /// غیرفعال (حذف منطقی یا تعلیق).
    /// </summary>
    Inactive = 1
}