using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Events;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// Aggregate Root اختیار سازمانی — اختیار سازمانی/امضایی پست‌ها.
/// </summary>
/// <remarks>
/// قواعد دامنه‌ای:
/// <list type="bullet">
/// <item>اختیار از مسئولیت جداست و به Signing محدود نیست (ADR-011).</item>
/// <item><c>Code</c> یکتا درون مستأجر است.</item>
/// <item>اختیار به Post منتسب می‌شود؛ Scope از طریق <c>OrganizationId</c> روی Assignment.</item>
/// <item>پایان انتساب با تاریخ و غیرفعال‌سازی است، نه حذف فیزیکی.</item>
/// </list>
/// </remarks>
public sealed class Authority : FullAuditableEntity
{
    private readonly List<PostAuthorityAssignment> _assignments = new();

    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private Authority()
    {
    }

    /// <summary>
    /// کد یکتای اختیار درون مستأجر.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// عنوان اختیار.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// شرح اختیاری.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// آیا اختیار فعال است؟
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// انتساب‌های این اختیار به پست‌ها.
    /// </summary>
    public IReadOnlyCollection<PostAuthorityAssignment> Assignments => _assignments.AsReadOnly();

    /// <summary>
    /// تعریف اختیار جدید.
    /// </summary>
    /// <param name="id">شناسه اختیار</param>
    /// <param name="tenantId">شناسه مستأجر</param>
    /// <param name="code">کد یکتا</param>
    /// <param name="title">عنوان</param>
    /// <param name="description">شرح اختیاری</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <returns>اختیار ایجادشده</returns>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن ورودی‌ها</exception>
    public static Authority Create(
        Guid id,
        Guid tenantId,
        string code,
        string title,
        string? description,
        DateTimeOffset occurredOn)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("شناسه اختیار معتبر نیست.", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مستأجر معتبر نیست.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("کد اختیار نمی‌تواند خالی باشد.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("عنوان اختیار نمی‌تواند خالی باشد.", nameof(title));
        }

        var authority = new Authority
        {
            Id = id,
            TenantId = tenantId,
            Code = code.Trim(),
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsActive = true
        };

        authority.AddDomainEvent(
            new AuthorityCreated(id, tenantId, authority.Code, occurredOn));
        return authority;
    }

    /// <summary>
    /// ویرایش عنوان و شرح اختیار.
    /// </summary>
    public void UpdateDetails(string title, string? description, DateTimeOffset occurredOn)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("عنوان اختیار نمی‌تواند خالی باشد.", nameof(title));
        }

        var newTitle = title.Trim();
        var newDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (Title == newTitle && Description == newDescription)
        {
            return;
        }

        Title = newTitle;
        Description = newDescription;
        AddDomainEvent(new AuthorityUpdated(Id, occurredOn));
    }

    /// <summary>
    /// غیرفعال‌سازی اختیار.
    /// </summary>
    public void Deactivate(DateTimeOffset occurredOn)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        AddDomainEvent(new AuthorityDeactivated(Id, occurredOn));
    }

    /// <summary>
    /// انتساب اختیار به پست در Scope سازمانی.
    /// </summary>
    /// <param name="organizationId">شناسه سازمان (مرجع IAM)</param>
    /// <param name="postId">شناسه پست مقصد</param>
    /// <param name="startDate">تاریخ شروع (اختیاری)</param>
    /// <param name="endDate">تاریخ پایان (اختیاری)</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <returns>انتساب ایجادشده (برای ثبت صریح در persistence)</returns>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن ورودی‌ها</exception>
    /// <exception cref="InvalidOperationException">در صورت انتساب جاری تکراری</exception>
    public PostAuthorityAssignment AssignToPost(
        Guid organizationId,
        Guid postId,
        DateOnly? startDate,
        DateOnly? endDate,
        DateTimeOffset occurredOn)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("شناسه سازمان معتبر نیست.", nameof(organizationId));
        }

        if (postId == Guid.Empty)
        {
            throw new ArgumentException("شناسه پست معتبر نیست.", nameof(postId));
        }

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            throw new ArgumentException("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.", nameof(startDate));
        }

        if (_assignments.Any(a => a.PostId == postId && a.OrganizationId == organizationId && a.IsCurrent))
        {
            throw new InvalidOperationException("انتساب جاری این اختیار به پست از قبل وجود دارد.");
        }

        var assignment = PostAuthorityAssignment.Create(Id, organizationId, postId, startDate, endDate);
        assignment.TenantId = TenantId;
        _assignments.Add(assignment);
        AddDomainEvent(new AuthorityAssigned(Id, postId, organizationId, occurredOn));
        return assignment;
    }

    /// <summary>
    /// پایان دادن به انتساب جاری اختیار به پست.
    /// </summary>
    /// <param name="assignmentId">شناسه انتساب</param>
    /// <param name="endDate">تاریخ پایان</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="InvalidOperationException">در صورت نبود انتساب جاری</exception>
    public void EndAssignment(Guid assignmentId, DateOnly endDate, DateTimeOffset occurredOn)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId && a.IsCurrent);

        if (assignment is null)
        {
            throw new InvalidOperationException("انتساب جاری یافت نشد.");
        }

        assignment.End(endDate);
        AddDomainEvent(new AuthorityAssignmentEnded(Id, assignment.PostId, occurredOn));
    }
}