using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// انتساب اختیار به پست — موجودیت عضو Aggregate <c>Authority</c>.
/// </summary>
/// <remarks>
/// پایان انتساب با <c>EndDate</c> و <c>IsActive=false</c> ثبت می‌شود، نه حذف فیزیکی.
/// </remarks>
public sealed class PostAuthorityAssignment : TenantEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private PostAuthorityAssignment()
    {
    }

    /// <summary>
    /// شناسه اختیار مالک انتساب.
    /// </summary>
    public Guid AuthorityId { get; private set; }

    /// <summary>
    /// شناسه سازمان (Scope انتساب؛ مرجع IAM).
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// شناسه پست مقصد (بدون ناوبری برای حفظ مرز Aggregate).
    /// </summary>
    public Guid PostId { get; private set; }

    /// <summary>
    /// تاریخ شروع انتساب (اختیاری).
    /// </summary>
    public DateOnly? StartDate { get; private set; }

    /// <summary>
    /// تاریخ پایان انتساب (خالی یعنی جاری).
    /// </summary>
    public DateOnly? EndDate { get; private set; }

    /// <summary>
    /// آیا انتساب فعال است؟
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// آیا انتساب در حال حاضر جاری است؟ (فعال، پایان‌نیافته و حذف‌نشده)
    /// </summary>
    public bool IsCurrent => IsActive && !IsDeleted && EndDate is null;

    /// <summary>
    /// ساخت انتساب جدید (فقط از طریق Aggregate Authority).
    /// </summary>
    internal static PostAuthorityAssignment Create(
        Guid authorityId,
        Guid organizationId,
        Guid postId,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        return new PostAuthorityAssignment
        {
            Id = Guid.NewGuid(),
            AuthorityId = authorityId,
            OrganizationId = organizationId,
            PostId = postId,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };
    }

    /// <summary>
    /// پایان دادن به انتساب در تاریخ مشخص.
    /// </summary>
    /// <param name="endDate">تاریخ پایان</param>
    internal void End(DateOnly endDate)
    {
        EndDate = endDate;
        IsActive = false;
    }
}