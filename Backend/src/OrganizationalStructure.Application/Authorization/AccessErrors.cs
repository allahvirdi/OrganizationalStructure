using OrganizationalStructure.Application.Common;

namespace OrganizationalStructure.Application.Authorization;

/// <summary>
/// خطاهای دسترسی (Authorization) برای Error Contract یکپارچه.
/// </summary>
public static class AccessErrors
{
    /// <summary>
    /// ساخت خطای «دسترسی به محدوده سازمانی مجاز نیست».
    /// </summary>
    public static Error Forbidden() => new(
        "Access.Forbidden",
        "دسترسی به این محدوده سازمانی مجاز نیست.",
        ErrorType.Forbidden);
}