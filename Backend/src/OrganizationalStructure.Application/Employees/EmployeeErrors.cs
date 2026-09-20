using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Employees;

/// <summary>
/// کدهای خطای دامنه پرسنل برای Error Contract یکپارچه.
/// </summary>
public static class EmployeeErrors
{
    /// <summary>
    /// ساخت خطای «پرسنل یافت نشد».
    /// </summary>
    public static Error NotFound(Guid employeeId) => new(
        "Employee.NotFound",
        $"پرسنل با شناسه {employeeId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «کد پرسنلی تکراری».
    /// </summary>
    public static Error DuplicatePersonnelCode(string code) => new(
        "Employee.DuplicatePersonnelCode",
        $"پرسنلی با کد {code} از قبل وجود دارد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «پست یافت نشد».
    /// </summary>
    public static Error PostNotFound(Guid postId) => new(
        "Employee.PostNotFound",
        $"پست با شناسه {postId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «تعارض انتساب».
    /// </summary>
    public static Error AssignmentConflict(string message) => new(
        "Employee.AssignmentConflict",
        message,
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «انتساب فعال وجود ندارد».
    /// </summary>
    public static Error NoActiveAssignment() => new(
        "Employee.NoActiveAssignment",
        "انتساب فعالی به این پست وجود ندارد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «پست متعلق به سازمان پرسنل نیست» (ADR-004 + ADR-013).
    /// </summary>
    public static Error PostOrganizationMismatch() => new(
        "Employee.PostOrganizationMismatch",
        "پست انتخاب‌شده متعلق به سازمان پرسنل نیست؛ فقط پست‌های همان سازمان قابل انتساب‌اند.",
        ErrorType.Conflict);
}