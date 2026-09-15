using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// انتساب مسئولیت به پست — موجودیت عضو Aggregate <c>Responsibility</c>.
/// </summary>
/// <remarks>
/// مسئولیت به Post متصل می‌شود، نه به User؛ با تغییر شاغل پست، مسئولیت برای فرد جدید برقرار می‌ماند.
/// پایان انتساب با <c>EndDate</c> و <c>IsActive=false</c> ثبت می‌شود، نه حذف فیزیکی.
/// </remarks>
public sealed class PostResponsibilityAssignment : TenantEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private PostResponsibilityAssignment()
    {
    }

    /// <summary>
    /// شناسه مسئولیت مالک انتساب.
    /// </summary>
    public Guid ResponsibilityId { get; private set; }

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
    /// ساخت انتساب جدید (فقط از طریق Aggregate Responsibility).
    /// </summary>
    internal static PostResponsibilityAssignment Create(
        Guid responsibilityId,
        Guid organizationId,
        Guid postId,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        return new PostResponsibilityAssignment
        {
            Id = Guid.NewGuid(),
            ResponsibilityId = responsibilityId,
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