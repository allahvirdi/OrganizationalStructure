namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// موجودیت پایه دارای ویژگی‌های حسابرسی.
/// </summary>
/// <remarks>
/// از <see cref="BaseEntity"/> مشتق می‌شود و موقعیت دوم زنجیره را تشکیل می‌دهد:
/// <code>BaseEntity → AuditableEntity → TenantEntity → FullAuditableEntity</code>.
/// </remarks>
public abstract class AuditableEntity : BaseEntity, IAuditable
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public Guid CreatedById { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public Guid? UpdatedById { get; set; }
}