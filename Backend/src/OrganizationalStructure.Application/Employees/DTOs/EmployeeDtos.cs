namespace OrganizationalStructure.Application.Employees.DTOs;

/// <summary>
/// DTO پرسنل.
/// </summary>
/// <remarks>
/// شامل فیلدهای PII است؛ کنترل دسترسی به داده حساس با Permission
/// <c>OrganizationStructure.Employee.ViewSensitiveData</c> در Phase 4 اعمال می‌شود.
/// </remarks>
public sealed record EmployeeDto
{
    /// <summary>
    /// شناسه پرسنل.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// کد پرسنلی (۸ رقمی).
    /// </summary>
    public string PersonnelCode { get; init; } = string.Empty;

    /// <summary>
    /// نام.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// نام خانوادگی.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// کد ملی.
    /// </summary>
    public string NationalCode { get; init; } = string.Empty;

    /// <summary>
    /// موبایل.
    /// </summary>
    public string? Mobile { get; init; }

    /// <summary>
    /// تاریخ تولد.
    /// </summary>
    public DateOnly? BirthDate { get; init; }

    /// <summary>
    /// سال سابقه حراست.
    /// </summary>
    public int? ServiceYears { get; init; }

    /// <summary>
    /// ماه سابقه حراست.
    /// </summary>
    public int? ServiceMonths { get; init; }

    /// <summary>
    /// موبایل پژواک.
    /// </summary>
    public string? PezhvakMobile { get; init; }

    /// <summary>
    /// شناسه کاربر IAM.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// آیا فعال است؟
    /// </summary>
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO پستِ منتسب به پرسنل (همراه مشخصات نمایشی پست).
/// </summary>
public sealed record EmployeePostDto
{
    /// <summary>
    /// شناسه پست.
    /// </summary>
    public Guid PostId { get; init; }

    /// <summary>
    /// کد پست.
    /// </summary>
    public string PostCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست.
    /// </summary>
    public string PostTitle { get; init; } = string.Empty;

    /// <summary>
    /// تاریخ شروع.
    /// </summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// تاریخ پایان.
    /// </summary>
    public DateOnly? ToDate { get; init; }

    /// <summary>
    /// آیا انتساب اصلی است؟
    /// </summary>
    public bool IsPrimary { get; init; }
}

/// <summary>
/// DTO پرسنلِ منتسب به پست.
/// </summary>
public sealed record PostEmployeeDto
{
    /// <summary>
    /// شناسه پرسنل.
    /// </summary>
    public Guid EmployeeId { get; init; }

    /// <summary>
    /// کد پرسنلی.
    /// </summary>
    public string PersonnelCode { get; init; } = string.Empty;

    /// <summary>
    /// نام کامل.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// تاریخ شروع.
    /// </summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// تاریخ پایان.
    /// </summary>
    public DateOnly? ToDate { get; init; }

    /// <summary>
    /// آیا انتساب اصلی است؟
    /// </summary>
    public bool IsPrimary { get; init; }
}