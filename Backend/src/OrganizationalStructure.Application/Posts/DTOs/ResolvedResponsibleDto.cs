namespace OrganizationalStructure.Application.Posts.DTOs;

/// <summary>
/// DTO نتیجه مسیریابی مسئولیت (FindResponsible).
/// </summary>
/// <remarks>
/// زنجیره: Responsibility → Assignment جاری → Post → Active Employee → IAM User.
/// </remarks>
public sealed record ResolvedResponsibleDto
{
    /// <summary>
    /// شناسه مسئولیت.
    /// </summary>
    public Guid ResponsibilityId { get; init; }

    /// <summary>
    /// کد مسئولیت.
    /// </summary>
    public string ResponsibilityCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان مسئولیت.
    /// </summary>
    public string ResponsibilityTitle { get; init; } = string.Empty;

    /// <summary>
    /// شناسه سازمان (Scope انتساب).
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// شناسه پست مسئول.
    /// </summary>
    public Guid PostId { get; init; }

    /// <summary>
    /// کد پست مسئول.
    /// </summary>
    public string PostCode { get; init; } = string.Empty;

    /// <summary>
    /// عنوان پست مسئول.
    /// </summary>
    public string PostTitle { get; init; } = string.Empty;

    /// <summary>
    /// پرسنل فعال منتسب به پست مسئول.
    /// </summary>
    public IReadOnlyList<ResolvedEmployeeDto> Employees { get; init; } =
        Array.Empty<ResolvedEmployeeDto>();
}

/// <summary>
/// DTO خلاصه پرسنل فعال در نتیجه مسیریابی.
/// </summary>
public sealed record ResolvedEmployeeDto
{
    /// <summary>
    /// شناسه پرسنل در سامانه ساختار سازمانی.
    /// </summary>
    public Guid EmployeeId { get; init; }

    /// <summary>
    /// شناسه کاربر در IAM (اختیاری — ممکن است هنوز متصل نشده باشد).
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// کد پرسنلی.
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
    /// آیا این انتساب، انتساب اصلی پرسنل است؟
    /// </summary>
    public bool IsPrimary { get; init; }
}
