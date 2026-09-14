namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// موجودیت پایه وابسته به مستأجر با پشتیبانی Soft Delete.
/// </summary>
/// <remarks>
/// از <see cref="AuditableEntity"/> مشتق می‌شود و موقعیت سوم زنجیره را تشکیل می‌دهد:
/// <code>BaseEntity → AuditableEntity → TenantEntity → FullAuditableEntity</code>.
/// Global Query Filter برای <see cref="TenantId"/> و <see cref="IsDeleted"/>
/// توسط DbContext در لایه زیرساخت اعمال می‌شود.
/// </remarks>
public abstract class TenantEntity : AuditableEntity, ITenantScoped
{
    /// <inheritdoc />
    public Guid TenantId { get; set; }

    /// <summary>
    /// پرچم Soft Delete؛ هنگام true رکورد از کوئری‌ها حذف می‌شود.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// زمان Soft Delete شدن رکورد.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// شناسه کاربر حذف‌کننده رکورد.
    /// </summary>
    public Guid? DeletedById { get; set; }
}