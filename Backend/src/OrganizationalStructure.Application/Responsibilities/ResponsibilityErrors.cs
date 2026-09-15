using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Responsibilities;

/// <summary>
/// کدهای خطای دامنه مسئولیت.
/// </summary>
public static class ResponsibilityErrors
{
    /// <summary>
    /// ساخت خطای «مسئولیت یافت نشد».
    /// </summary>
    public static Error NotFound(Guid id) => new(
        "Responsibility.NotFound",
        $"مسئولیت با شناسه {id} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «مسئولیت با کد یافت نشد».
    /// </summary>
    public static Error NotFoundByCode(string code) => new(
        "Responsibility.NotFound",
        $"مسئولیت با کد {code} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «کد تکراری».
    /// </summary>
    public static Error DuplicateCode(string code) => new(
        "Responsibility.DuplicateCode",
        $"مسئولیتی با کد {code} از قبل وجود دارد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «پست یافت نشد».
    /// </summary>
    public static Error PostNotFound(Guid postId) => new(
        "Responsibility.PostNotFound",
        $"پست با شناسه {postId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «مسئولیت غیرفعال».
    /// </summary>
    public static Error Inactive(string code) => new(
        "Responsibility.Inactive",
        $"مسئولیت {code} غیرفعال است و قابل انتساب نیست.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «تعارض انتساب».
    /// </summary>
    public static Error AssignmentConflict(string message) => new(
        "Responsibility.AssignmentConflict",
        message,
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «انتساب جاری یافت نشد».
    /// </summary>
    public static Error AssignmentNotFound(Guid assignmentId) => new(
        "Responsibility.AssignmentNotFound",
        $"انتساب جاری با شناسه {assignmentId} یافت نشد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «غیرفعال‌سازی با انتساب فعال».
    /// </summary>
    public static Error HasActiveAssignments() => new(
        "Responsibility.HasActiveAssignments",
        "مسئولیت دارای انتساب جاری است؛ ابتدا انتساب‌ها را پایان دهید.",
        ErrorType.Conflict);
}