using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Authorities;

/// <summary>
/// کدهای خطای دامنه اختیار.
/// </summary>
public static class AuthorityErrors
{
    /// <summary>
    /// ساخت خطای «اختیار یافت نشد».
    /// </summary>
    public static Error NotFound(Guid id) => new(
        "Authority.NotFound",
        $"اختیار با شناسه {id} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «اختیار با کد یافت نشد».
    /// </summary>
    public static Error NotFoundByCode(string code) => new(
        "Authority.NotFound",
        $"اختیار با کد {code} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «کد تکراری».
    /// </summary>
    public static Error DuplicateCode(string code) => new(
        "Authority.DuplicateCode",
        $"اختیاری با کد {code} از قبل وجود دارد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «پست یافت نشد».
    /// </summary>
    public static Error PostNotFound(Guid postId) => new(
        "Authority.PostNotFound",
        $"پست با شناسه {postId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «اختیار غیرفعال».
    /// </summary>
    public static Error Inactive(string code) => new(
        "Authority.Inactive",
        $"اختیار {code} غیرفعال است و قابل انتساب نیست.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «تعارض انتساب».
    /// </summary>
    public static Error AssignmentConflict(string message) => new(
        "Authority.AssignmentConflict",
        message,
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «انتساب جاری یافت نشد».
    /// </summary>
    public static Error AssignmentNotFound(Guid assignmentId) => new(
        "Authority.AssignmentNotFound",
        $"انتساب جاری با شناسه {assignmentId} یافت نشد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «غیرفعال‌سازی با انتساب فعال».
    /// </summary>
    public static Error HasActiveAssignments() => new(
        "Authority.HasActiveAssignments",
        "اختیار دارای انتساب جاری است؛ ابتدا انتساب‌ها را پایان دهید.",
        ErrorType.Conflict);
}