namespace OrganizationalStructure.Domain.Common;

/// <summary>
/// موجودیت پایه تمام موجودیت‌های دامنه.
/// </summary>
/// <remarks>
/// شامل شناسه و پرچم مورد استفاده در لایه‌های بالاتر است.
/// تمام Aggregate Rootها و موجودیت‌های این سامانه از این نوع مشتق می‌شوند.
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// شناسه یکتای موجودیت.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// رویدادهای دامنه‌ی در انتظار این موجودیت.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// دریافت رویدادهای دامنه‌ی در انتظار.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// افزودن رویداد دامنه به موجودیت.
    /// </summary>
    /// <param name="domainEvent">رویداد دامنه</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// پاک‌سازی رویدادهای دامنه پس از ثبت در پایگاه داده.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}