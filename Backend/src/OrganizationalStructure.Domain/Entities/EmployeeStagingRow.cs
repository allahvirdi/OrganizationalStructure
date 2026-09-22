using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Encryption;
using OrganizationalStructure.Domain.Enums;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// ردیف واسط پرسنل — داده خام قبل از تأیید و ثبت نهایی.
/// </summary>
/// <remarks>
/// مطابق DEC-030: بدون TenantId، بدون PezhvakMobile، بدون Soft Delete (حذف فیزیکی پس از ۱۰ روز).
/// ستون‌های PII دقیقاً مانند Employees رمزنگاری می‌شوند (ADR-006).
/// </remarks>
public sealed class EmployeeStagingRow : BaseEntity
{
    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private EmployeeStagingRow()
    {
    }

    /// <summary>
    /// شناسه بارگذاری والد.
    /// </summary>
    public Guid BatchId { get; private set; }

    /// <summary>
    /// شماره ردیف در فایل / ترتیب درج.
    /// </summary>
    public int RowNumber { get; private set; }

    /// <summary>
    /// کد پرسنلی: عدد ۸ رقمی.
    /// </summary>
    public string PersonnelCode { get; private set; } = string.Empty;

    /// <summary>
    /// نام.
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری تصادفی (Randomized).</remarks>
    [PiiEncrypted(EncryptionType.Randomized)]
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// نام خانوادگی.
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری تصادفی (Randomized).</remarks>
    [PiiEncrypted(EncryptionType.Randomized)]
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// کد ملی.
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری قطعی (Deterministic) برای جستجو.</remarks>
    [PiiEncrypted(EncryptionType.Deterministic)]
    public string NationalCode { get; private set; } = string.Empty;

    /// <summary>
    /// شماره موبایل (اختیاری).
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری قطعی (Deterministic).</remarks>
    [PiiEncrypted(EncryptionType.Deterministic)]
    public string? Mobile { get; private set; }

    /// <summary>
    /// تاریخ تولد (اختیاری؛ میلادی پس از تبدیل از شمسی).
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری تصادفی (Randomized).</remarks>
    [PiiEncrypted(EncryptionType.Randomized)]
    public DateOnly? BirthDate { get; private set; }

    /// <summary>
    /// سال‌های سابقه خدمت.
    /// </summary>
    public int? ServiceYears { get; private set; }

    /// <summary>
    /// ماه‌های سابقه خدمت (۰..۱۱).
    /// </summary>
    public int? ServiceMonths { get; private set; }

    /// <summary>
    /// وضعیت اعتبارسنجی ردیف.
    /// </summary>
    public StagingRowValidationStatus ValidationStatus { get; private set; }

    /// <summary>
    /// شناسه Employee ایجادشده پس از Commit.
    /// </summary>
    public Guid? EmployeeId { get; private set; }

    /// <summary>
    /// وضعیت ثبت نهایی.
    /// </summary>
    public StagingRowCommitStatus CommitStatus { get; private set; }

    /// <summary>
    /// زمان ایجاد ردیف.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// ایجاد ردیف واسط جدید.
    /// </summary>
    /// <param name="id">شناسه ردیف</param>
    /// <param name="batchId">شناسه بارگذاری</param>
    /// <param name="rowNumber">شماره ردیف</param>
    /// <param name="personnelCode">کد پرسنلی</param>
    /// <param name="firstName">نام</param>
    /// <param name="lastName">نام خانوادگی</param>
    /// <param name="nationalCode">کد ملی</param>
    /// <param name="mobile">موبایل (اختیاری)</param>
    /// <param name="birthDate">تاریخ تولد (اختیاری)</param>
    /// <param name="serviceYears">سال سابقه (اختیاری)</param>
    /// <param name="serviceMonths">ماه سابقه (اختیاری)</param>
    /// <param name="createdAt">زمان ایجاد</param>
    /// <returns>ردیف واسط ایجادشده</returns>
    public static EmployeeStagingRow Create(
        Guid id,
        Guid batchId,
        int rowNumber,
        string personnelCode,
        string firstName,
        string lastName,
        string nationalCode,
        string? mobile,
        DateOnly? birthDate,
        int? serviceYears,
        int? serviceMonths,
        DateTimeOffset createdAt)
    {
        return new EmployeeStagingRow
        {
            Id = id,
            BatchId = batchId,
            RowNumber = rowNumber,
            PersonnelCode = personnelCode,
            FirstName = firstName,
            LastName = lastName,
            NationalCode = nationalCode,
            Mobile = mobile,
            BirthDate = birthDate,
            ServiceYears = serviceYears,
            ServiceMonths = serviceMonths,
            ValidationStatus = StagingRowValidationStatus.Pending,
            CommitStatus = StagingRowCommitStatus.Pending,
            CreatedAt = createdAt
        };
    }

    /// <summary>
    /// تعیین وضعیت اعتبارسنجی.
    /// </summary>
    /// <param name="status">وضعیت جدید</param>
    public void SetValidationStatus(StagingRowValidationStatus status)
    {
        ValidationStatus = status;
    }

    /// <summary>
    /// ثبت نهایی ردیف (لینک به Employee ایجادشده).
    /// </summary>
    /// <param name="employeeId">شناسه Employee</param>
    public void MarkCommitted(Guid employeeId)
    {
        EmployeeId = employeeId;
        CommitStatus = StagingRowCommitStatus.Committed;
    }

    /// <summary>
    /// علامت‌گذاری به‌عنوان ردشده.
    /// </summary>
    public void MarkSkipped()
    {
        CommitStatus = StagingRowCommitStatus.Skipped;
    }
}