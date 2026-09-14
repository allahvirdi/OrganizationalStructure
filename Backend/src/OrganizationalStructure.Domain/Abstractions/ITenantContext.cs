namespace OrganizationalStructure.Domain.Abstractions;

/// <summary>
/// دسترسی به شناسه مستأجر جاری برای جداسازی داده (Multi-tenancy).
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// شناسه مستأجر جاری.
    /// </summary>
    Guid TenantId { get; }
}