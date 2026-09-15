using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// Aggregate Root پست سازمانی — یک گره در درخت ساختار سازمانی و جایگاه فرد در ساختار.
/// </summary>
/// <remarks>
/// قواعد دامنه‌ای:
/// <list type="bullet">
/// <item>هر Post متعلق به دقیقاً یک Organization است و <c>OrganizationId</c> پس از ایجاد تغییر نمی‌کند (ADR-004).</item>
/// <item>هر Post حداکثر یک والد مستقیم (<c>ParentId</c>) و صفر یا چند فرزند مستقیم دارد.</item>
/// <item>پست نمی‌تواند والد خودش باشد؛ جلوگیری از چرخه‌های چندسطحی در لایه کاربرد (با دسترسی به درخت) انجام می‌شود.</item>
/// <item>جابجایی بین Organizationها ممنوع است (ADR-004).</item>
/// <item>مسئولیت و اختیار، مفاهیم مستقل‌اند و از طریق Assignment به Post متصل می‌شوند، نه به‌صورت فیلد (ADR-011).</item>
/// <item>غیرفعال‌سازی به معنی حذف تاریخی نیست.</item>
/// </list>
/// </remarks>
public sealed class Post : FullAuditableEntity
{
    private readonly List<Post> _children = new();

    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private Post()
    {
    }

    /// <summary>
    /// شناسه سازمان مالک درخت (مرجع IAM — بدون FK فیزیکی).
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// کد یکتای پست درون سازمان.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// شرح اختیاری پست.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// شناسه والد مستقیم (خالی یعنی ریشه درخت سازمان).
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// والد مستقیم (ناوبری).
    /// </summary>
    public Post? Parent { get; private set; }

    /// <summary>
    /// فرزندان مستقیم (ناوبری).
    /// </summary>
    public IReadOnlyCollection<Post> Children => _children.AsReadOnly();

    /// <summary>
    /// آیا پست فعال است؟
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// ایجاد پست سازمانی جدید.
    /// </summary>
    /// <param name="id">شناسه پست</param>
    /// <param name="tenantId">شناسه مستأجر</param>
    /// <param name="organizationId">شناسه سازمان (مرجع IAM)</param>
    /// <param name="code">کد یکتای پست درون سازمان</param>
    /// <param name="title">عنوان پست</param>
    /// <param name="description">شرح اختیاری</param>
    /// <param name="parentId">شناسه والد مستقیم (خالی یعنی ریشه)</param>
    /// <param name="occurredOn">زمان وقوع (از ساعت تزریقی لایه کاربرد)</param>
    /// <returns>پست ایجادشده</returns>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن ورودی‌ها یا خودارجاعی والد</exception>
    public static Post Create(
        Guid id,
        Guid tenantId,
        Guid organizationId,
        string code,
        string title,
        string? description,
        Guid? parentId,
        DateTimeOffset occurredOn)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("شناسه پست معتبر نیست.", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مستأجر معتبر نیست.", nameof(tenantId));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("شناسه سازمان معتبر نیست.", nameof(organizationId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("کد پست نمی‌تواند خالی باشد.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("عنوان پست نمی‌تواند خالی باشد.", nameof(title));
        }

        if (parentId == id)
        {
            throw new ArgumentException("پست نمی‌تواند والد خودش باشد.", nameof(parentId));
        }

        var post = new Post
        {
            Id = id,
            TenantId = tenantId,
            OrganizationId = organizationId,
            Code = code.Trim(),
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ParentId = parentId,
            IsActive = true
        };

        post.AddDomainEvent(new PostCreated(id, tenantId, organizationId, post.Code, occurredOn));
        return post;
    }

    /// <summary>
    /// ویرایش عنوان و شرح پست.
    /// </summary>
    /// <param name="title">عنوان جدید</param>
    /// <param name="description">شرح جدید</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت خالی بودن عنوان</exception>
    public void UpdateDetails(string title, string? description, DateTimeOffset occurredOn)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("عنوان پست نمی‌تواند خالی باشد.", nameof(title));
        }

        var newTitle = title.Trim();
        var newDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (Title == newTitle && Description == newDescription)
        {
            return;
        }

        Title = newTitle;
        Description = newDescription;
        AddDomainEvent(new PostUpdated(Id, occurredOn));
    }

    /// <summary>
    /// تغییر کد پست (یکتایی درون سازمان در لایه کاربرد/پایگاه داده کنترل می‌شود).
    /// </summary>
    /// <param name="code">کد جدید</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت خالی بودن کد</exception>
    public void ChangeCode(string code, DateTimeOffset occurredOn)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("کد پست نمی‌تواند خالی باشد.", nameof(code));
        }

        var newCode = code.Trim();
        if (Code == newCode)
        {
            return;
        }

        Code = newCode;
        AddDomainEvent(new PostUpdated(Id, occurredOn));
    }

    /// <summary>
    /// تغییر والد مستقیم (جابجایی درون همان درخت/سازمان).
    /// </summary>
    /// <param name="parentId">شناسه والد جدید (خالی یعنی ریشه)</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت خودارجاعی</exception>
    /// <remarks>
    /// کنترل هم‌سازمانی بودن و نبود چرخه چندسطحی نیازمند دسترسی به درخت است و در لایه کاربرد انجام می‌شود.
    /// </remarks>
    public void ChangeParent(Guid? parentId, DateTimeOffset occurredOn)
    {
        if (parentId == Id)
        {
            throw new ArgumentException("پست نمی‌تواند والد خودش باشد.", nameof(parentId));
        }

        if (ParentId == parentId)
        {
            return;
        }

        var oldParentId = ParentId;
        ParentId = parentId;
        AddDomainEvent(new PostMoved(Id, oldParentId, parentId, occurredOn));
    }

    /// <summary>
    /// فعال‌سازی پست.
    /// </summary>
    /// <param name="occurredOn">زمان وقوع</param>
    public void Activate(DateTimeOffset occurredOn)
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        AddDomainEvent(new PostActivated(Id, occurredOn));
    }

    /// <summary>
    /// غیرفعال‌سازی پست (به معنی حذف تاریخی نیست).
    /// </summary>
    /// <param name="occurredOn">زمان وقوع</param>
    public void Deactivate(DateTimeOffset occurredOn)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        AddDomainEvent(new PostDeactivated(Id, occurredOn));
    }
}