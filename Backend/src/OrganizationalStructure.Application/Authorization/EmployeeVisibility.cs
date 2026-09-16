namespace OrganizationalStructure.Application.Authorization;

/// <summary>
/// قواعد مشاهده پرسنل بر اساس Scope سازمانی.
/// </summary>
/// <remarks>
/// پرسنل در Scope است اگر انتسابی نداشته باشد (تازه/بدون پست — قابل مدیریت)
/// یا حداقل یک انتساب جاری به پستی داخل Scope داشته باشد.
/// </remarks>
public static class EmployeeVisibility
{
    /// <summary>
    /// بررسی مشاهده‌پذیری پرسنل.
    /// </summary>
    /// <param name="assignedPostOrganizationIds">شناسه سازمان پست‌های دارای انتساب جاری</param>
    /// <param name="hasAnyAssignment">آیا هیچ ردیف انتسابی (جاری یا پایان‌یافته) دارد؟</param>
    /// <param name="scope">محدوده سازمانی کاربر</param>
    /// <returns>درست در صورت مشاهده‌پذیر بودن</returns>
    public static bool IsVisible(
        IEnumerable<Guid> assignedPostOrganizationIds,
        bool hasAnyAssignment,
        IReadOnlySet<Guid> scope)
    {
        if (!hasAnyAssignment)
        {
            return true;
        }

        return assignedPostOrganizationIds.Any(scope.Contains);
    }
}