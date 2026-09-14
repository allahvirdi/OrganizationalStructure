namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// قرارداد موجودیت وابسته به مستأجر (Multi-tenancy).
/// </summary>
public interface ITenantScoped
{
    /// <summary>
    /// شناسه مستأجر مربوط به رکورد.
    /// </summary>
    Guid TenantId { get; set; }
}