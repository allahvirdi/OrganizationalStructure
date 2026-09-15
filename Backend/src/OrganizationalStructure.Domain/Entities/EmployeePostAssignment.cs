using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// انتساب پرسنل به پست — موجودیت عضو Aggregate <c>Employee</c>.
/// </summary>
/// <remarks>
/// یک پرسنل می‌تواند هم‌زمان به چند پست منتسب باشد (رابطه یک‌به‌یک نیست).
/// نمونه‌سازی فقط از طریق متدهای <c>Employee</c> انجام می‌شود تا ناورداهای دامنه حفظ شود.
/// </remarks>
public sealed class EmployeePostAssignment : TenantEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private EmployeePostAssignment()
    {
    }

    /// <summary>
    /// شناسه پرسنل مالک انتساب.
    /// </summary>
    public Guid EmployeeId { get; private set; }

    /// <summary>
    /// شناسه پست مقصد (بدون ناوبری برای حفظ مرز Aggregate).
    /// </summary>
    public Guid PostId { get; private set; }

    /// <summary>
    /// تاریخ شروع انتساب (اختیاری).
    /// </summary>
    public DateOnly? FromDate { get; private set; }

    /// <summary>
    /// تاریخ پایان انتساب (خالی یعنی جاری).
    /// </summary>
    public DateOnly? ToDate { get; private set; }

    /// <summary>
    /// آیا این انتساب، انتساب اصلی پرسنل است؟
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// آیا انتساب در حال حاضر فعال است؟ (پایان‌نیافته و حذف‌نشده)
    /// </summary>
    public bool IsActiveAssignment => !IsDeleted && ToDate is null;

    /// <summary>
    /// ساخت انتساب جدید (فقط از طریق Aggregate Employee).
    /// </summary>
    internal static EmployeePostAssignment Create(
        Guid employeeId,
        Guid postId,
        DateOnly? fromDate,
        DateOnly? toDate,
        bool isPrimary)
    {
        return new EmployeePostAssignment
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            PostId = postId,
            FromDate = fromDate,
            ToDate = toDate,
            IsPrimary = isPrimary
        };
    }

    /// <summary>
    /// پایان دادن به انتساب در تاریخ مشخص.
    /// </summary>
    /// <param name="endDate">تاریخ پایان</param>
    internal void End(DateOnly endDate)
    {
        ToDate = endDate;
        IsPrimary = false;
    }
}