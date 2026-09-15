using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Posts;

/// <summary>
/// کدهای خطای دامنه پست برای Error Contract یکپارچه.
/// </summary>
public static class PostErrors
{
    /// <summary>
    /// ساخت خطای «پست یافت نشد».
    /// </summary>
    public static Error NotFound(Guid postId) => new(
        "Post.NotFound",
        $"پست با شناسه {postId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «کد تکراری در سازمان».
    /// </summary>
    public static Error DuplicateCode(string code) => new(
        "Post.DuplicateCode",
        $"پستی با کد {code} در این سازمان از قبل وجود دارد.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «والد یافت نشد».
    /// </summary>
    public static Error ParentNotFound(Guid parentId) => new(
        "Post.ParentNotFound",
        $"پست والد با شناسه {parentId} یافت نشد.",
        ErrorType.NotFound);

    /// <summary>
    /// ساخت خطای «جابجایی بین سازمانی ممنوع».
    /// </summary>
    public static Error CrossOrganizationMove() => new(
        "Post.CrossOrganizationMove",
        "جابجایی پست بین سازمان‌های مختلف مجاز نیست.",
        ErrorType.Conflict);

    /// <summary>
    /// ساخت خطای «چرخه در درخت».
    /// </summary>
    public static Error CycleDetected() => new(
        "Post.CycleDetected",
        "این جابجایی باعث ایجاد چرخه در ساختار درختی می‌شود.",
        ErrorType.Conflict);
}