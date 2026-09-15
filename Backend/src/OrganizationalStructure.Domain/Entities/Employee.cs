using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Encryption;
using OrganizationalStructure.Domain.Events;
using OrganizationalStructure.Domain.ValueObjects;

namespace OrganizationalStructure.Domain.Entities;

/// <summary>
/// Aggregate Root پرسنل — مالک کامل اطلاعات پرسنلی در این دامنه.
/// </summary>
/// <remarks>
/// قواعد دامنه‌ای:
/// <list type="bullet">
/// <item><c>Employee ≠ IAM User</c>؛ ارتباط با حساب کاربری فقط از طریق <c>UserId</c> اختیاری است (ADR-003، DEC-012).</item>
/// <item>یک پرسنل می‌تواند هم‌زمان به یک یا چند پست منتسب باشد.</item>
/// <item>در هر لحظه حداکثر یک انتساب اصلیِ فعال مجاز است.</item>
/// <item>فیلدهای حساس با <see cref="PiiEncryptedAttribute"/> مشخص شده‌اند (ADR-006)؛ اعمال رمزنگاری در لایه زیرساخت است.</item>
/// </list>
/// </remarks>
public sealed class Employee : FullAuditableEntity
{
    private readonly List<EmployeePostAssignment> _assignments = new();

    /// <summary>
    /// سازنده موردنیاز EF Core.
    /// </summary>
    private Employee()
    {
    }

    /// <summary>
    /// شناسه کاربر متناظر در IAM (اختیاری؛ بدون FK فیزیکی).
    /// </summary>
    public Guid? UserId { get; private set; }

    /// <summary>
    /// کد پرسنلی (یکتا درون مستأجر).
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
    /// <remarks>داده حساس — رمزنگاری قطعی (Deterministic) برای جستجو.</remarks>
    [PiiEncrypted(EncryptionType.Deterministic)]
    public string? Mobile { get; private set; }

    /// <summary>
    /// تاریخ تولد (اختیاری).
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری تصادفی (Randomized)؛ فقط نمایشی/گزارشی (ADR-010).</remarks>
    [PiiEncrypted(EncryptionType.Randomized)]
    public DateOnly? BirthDate { get; private set; }

    /// <summary>
    /// شماره موبایل پژواک (اختیاری).
    /// </summary>
    /// <remarks>داده حساس — رمزنگاری قطعی (Deterministic) برای جستجو (ADR-010).</remarks>
    [PiiEncrypted(EncryptionType.Deterministic)]
    public string? PezhvakMobile { get; private set; }

    /// <summary>
    /// سابقه خدمت در حراست (اختیاری؛ غیر PII).
    /// </summary>
    public HerasatServiceRecord? ServiceRecord { get; private set; }

    /// <summary>
    /// آیا پرسنل فعال است؟
    /// </summary>
    /// <remarks>
    /// پایان همکاری/بازنشستگی/فوت/استعفا باعث غیرفعال‌سازی می‌شود، نه حذف اطلاعات (DEC-022).
    /// </remarks>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// انتساب‌های پست این پرسنل.
    /// </summary>
    public IReadOnlyCollection<EmployeePostAssignment> Assignments => _assignments.AsReadOnly();

    /// <summary>
    /// ثبت پرسنل جدید.
    /// </summary>
    /// <param name="id">شناسه پرسنل</param>
    /// <param name="tenantId">شناسه مستأجر</param>
    /// <param name="personnelCode">کد پرسنلی (یکتا درون مستأجر)</param>
    /// <param name="firstName">نام</param>
    /// <param name="lastName">نام خانوادگی</param>
    /// <param name="nationalCode">کد ملی</param>
    /// <param name="mobile">شماره موبایل (اختیاری)</param>
    /// <param name="userId">شناسه کاربر IAM (اختیاری)</param>
    /// <param name="occurredOn">زمان وقوع (از ساعت تزریقی لایه کاربرد)</param>
    /// <param name="birthDate">تاریخ تولد (اختیاری؛ ADR-010)</param>
    /// <param name="serviceRecord">سابقه خدمت در حراست (اختیاری؛ ADR-010)</param>
    /// <param name="pezhvakMobile">شماره موبایل پژواک (اختیاری؛ ADR-010)</param>
    /// <returns>پرسنل ایجادشده</returns>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن ورودی‌ها</exception>
    public static Employee Create(
        Guid id,
        Guid tenantId,
        string personnelCode,
        string firstName,
        string lastName,
        string nationalCode,
        string? mobile,
        Guid? userId,
        DateTimeOffset occurredOn,
        DateOnly? birthDate = null,
        HerasatServiceRecord? serviceRecord = null,
        string? pezhvakMobile = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("شناسه پرسنل معتبر نیست.", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("شناسه مستأجر معتبر نیست.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(personnelCode))
        {
            throw new ArgumentException("کد پرسنلی نمی‌تواند خالی باشد.", nameof(personnelCode));
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("نام نمی‌تواند خالی باشد.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("نام خانوادگی نمی‌تواند خالی باشد.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            throw new ArgumentException("کد ملی نمی‌تواند خالی باشد.", nameof(nationalCode));
        }

        var employee = new Employee
        {
            Id = id,
            TenantId = tenantId,
            PersonnelCode = personnelCode.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            NationalCode = nationalCode.Trim(),
            Mobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim(),
            UserId = userId,
            BirthDate = birthDate,
            ServiceRecord = serviceRecord,
            PezhvakMobile = string.IsNullOrWhiteSpace(pezhvakMobile) ? null : pezhvakMobile.Trim(),
            IsActive = true
        };

        employee.AddDomainEvent(new EmployeeCreated(id, tenantId, employee.PersonnelCode, occurredOn));
        return employee;
    }

    /// <summary>
    /// ویرایش اطلاعات پرسنلی.
    /// </summary>
    public void UpdatePersonalInfo(
        string firstName,
        string lastName,
        string nationalCode,
        string? mobile,
        DateTimeOffset occurredOn)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("نام نمی‌تواند خالی باشد.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("نام خانوادگی نمی‌تواند خالی باشد.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            throw new ArgumentException("کد ملی نمی‌تواند خالی باشد.", nameof(nationalCode));
        }

        var newFirstName = firstName.Trim();
        var newLastName = lastName.Trim();
        var newNationalCode = nationalCode.Trim();
        var newMobile = string.IsNullOrWhiteSpace(mobile) ? null : mobile.Trim();

        if (FirstName == newFirstName && LastName == newLastName
            && NationalCode == newNationalCode && Mobile == newMobile)
        {
            return;
        }

        FirstName = newFirstName;
        LastName = newLastName;
        NationalCode = newNationalCode;
        Mobile = newMobile;
        AddDomainEvent(new EmployeeUpdated(Id, occurredOn));
    }

    /// <summary>
    /// ویرایش اطلاعات تکمیلی پرسنل (تاریخ تولد، سابقه حراست، موبایل پژواک).
    /// </summary>
    /// <param name="birthDate">تاریخ تولد (اختیاری)</param>
    /// <param name="serviceRecord">سابقه خدمت در حراست (اختیاری)</param>
    /// <param name="pezhvakMobile">شماره موبایل پژواک (اختیاری)</param>
    /// <param name="occurredOn">زمان وقوع</param>
    public void UpdateSupplementaryInfo(
        DateOnly? birthDate,
        HerasatServiceRecord? serviceRecord,
        string? pezhvakMobile,
        DateTimeOffset occurredOn)
    {
        var newPezhvakMobile = string.IsNullOrWhiteSpace(pezhvakMobile) ? null : pezhvakMobile.Trim();

        if (BirthDate == birthDate && Equals(ServiceRecord, serviceRecord) && PezhvakMobile == newPezhvakMobile)
        {
            return;
        }

        BirthDate = birthDate;
        ServiceRecord = serviceRecord;
        PezhvakMobile = newPezhvakMobile;
        AddDomainEvent(new EmployeeUpdated(Id, occurredOn));
    }

    /// <summary>
    /// اتصال پرسنل به حساب کاربری IAM.
    /// </summary>
    /// <param name="userId">شناسه کاربر IAM</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن شناسه کاربر</exception>
    public void LinkToUser(Guid userId, DateTimeOffset occurredOn)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("شناسه کاربر معتبر نیست.", nameof(userId));
        }

        if (UserId == userId)
        {
            return;
        }

        UserId = userId;
        AddDomainEvent(new EmployeeUpdated(Id, occurredOn));
    }

    /// <summary>
    /// قطع ارتباط پرسنل از حساب کاربری IAM.
    /// </summary>
    /// <param name="occurredOn">زمان وقوع</param>
    public void UnlinkFromUser(DateTimeOffset occurredOn)
    {
        if (UserId is null)
        {
            return;
        }

        UserId = null;
        AddDomainEvent(new EmployeeUpdated(Id, occurredOn));
    }

    /// <summary>
    /// اصلاح کد پرسنلی (یکتایی درون مستأجر در لایه کاربرد/پایگاه داده کنترل می‌شود).
    /// </summary>
    /// <param name="personnelCode">کد پرسنلی جدید</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت خالی بودن کد پرسنلی</exception>
    public void ChangePersonnelCode(string personnelCode, DateTimeOffset occurredOn)
    {
        if (string.IsNullOrWhiteSpace(personnelCode))
        {
            throw new ArgumentException("کد پرسنلی نمی‌تواند خالی باشد.", nameof(personnelCode));
        }

        var newCode = personnelCode.Trim();
        if (PersonnelCode == newCode)
        {
            return;
        }

        PersonnelCode = newCode;
        AddDomainEvent(new EmployeeUpdated(Id, occurredOn));
    }

    /// <summary>
    /// انتساب پرسنل به پست.
    /// </summary>
    /// <param name="postId">شناسه پست مقصد</param>
    /// <param name="fromDate">تاریخ شروع (اختیاری)</param>
    /// <param name="toDate">تاریخ پایان (اختیاری؛ خالی یعنی جاری)</param>
    /// <param name="isPrimary">آیا انتساب اصلی است؟</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="ArgumentException">در صورت نامعتبر بودن ورودی‌ها</exception>
    /// <exception cref="InvalidOperationException">در صورت انتساب فعال تکراری یا اصلیِ دوم</exception>
    public void AssignToPost(
        Guid postId,
        DateOnly? fromDate,
        DateOnly? toDate,
        bool isPrimary,
        DateTimeOffset occurredOn)
    {
        if (postId == Guid.Empty)
        {
            throw new ArgumentException("شناسه پست معتبر نیست.", nameof(postId));
        }

        if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
        {
            throw new ArgumentException("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد.", nameof(fromDate));
        }

        if (_assignments.Any(a => a.PostId == postId && a.IsActiveAssignment))
        {
            throw new InvalidOperationException("انتساب فعال به این پست از قبل وجود دارد.");
        }

        if (isPrimary && _assignments.Any(a => a.IsPrimary && a.IsActiveAssignment))
        {
            throw new InvalidOperationException("پرسنل در حال حاضر یک انتساب اصلی فعال دارد؛ ابتدا آن را پایان دهید.");
        }

        var assignment = EmployeePostAssignment.Create(Id, postId, fromDate, toDate, isPrimary);
        assignment.TenantId = TenantId;
        _assignments.Add(assignment);
        AddDomainEvent(new EmployeeAssignedToPost(Id, postId, isPrimary, occurredOn));
    }

    /// <summary>
    /// پایان دادن به انتساب فعال پرسنل به پست.
    /// </summary>
    /// <param name="postId">شناسه پست</param>
    /// <param name="endDate">تاریخ پایان</param>
    /// <param name="occurredOn">زمان وقوع</param>
    /// <exception cref="InvalidOperationException">در صورت نبود انتساب فعال</exception>
    public void EndAssignment(Guid postId, DateOnly endDate, DateTimeOffset occurredOn)
    {
        var assignment = _assignments.FirstOrDefault(a => a.PostId == postId && a.IsActiveAssignment);

        if (assignment is null)
        {
            throw new InvalidOperationException("انتساب فعالی به این پست وجود ندارد.");
        }

        assignment.End(endDate);
        AddDomainEvent(new EmployeeAssignmentEnded(Id, postId, occurredOn));
    }

    /// <summary>
    /// فعال‌سازی پرسنل.
    /// </summary>
    public void Activate(DateTimeOffset occurredOn)
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        AddDomainEvent(new EmployeeActivated(Id, occurredOn));
    }

    /// <summary>
    /// غیرفعال‌سازی پرسنل (به معنی حذف تاریخی نیست).
    /// </summary>
    public void Deactivate(DateTimeOffset occurredOn)
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        AddDomainEvent(new EmployeeDeactivated(Id, occurredOn));
    }
}