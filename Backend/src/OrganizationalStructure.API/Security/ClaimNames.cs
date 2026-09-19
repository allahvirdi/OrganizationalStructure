namespace OrganizationalStructure.API.Security;

/// <summary>
/// نام ادعاهای (Claim) مورد استفاده از توکن احراز هویت IAM.
/// </summary>
/// <remarks>
/// این ثابت‌ها همراستا با <c>EnterpriseIAM.Domain.Constants.ClaimTypes</c> هستند.
/// نام <c>TenantId</c> بر اساس اسناد توکن IAM پروژه‌ی خواهر ثبت شده است؛
/// در صورت عدم حضور این ادعا در توکن، رفتار باید Fail-closed باشد (قابل بررسی Q-002 ادامه).
/// </remarks>
public static class ClaimNames
{
    /// <summary>
    /// شناسه کاربر (از IAM).
    /// </summary>
    public const string UserId = "user_id";

    /// <summary>
    /// نام کاربری.
    /// </summary>
    public const string UserName = "username";

    /// <summary>
    /// شناسه سازمان (از IAM).
    /// </summary>
    public const string OrganizationId = "organization_id";

    /// <summary>
    /// شناسه مستأجر.
    /// </summary>
    public const string TenantId = "tenant_id";

    /// <summary>
    /// نقش (از IAM).
    /// </summary>
    public const string Role = "role";

    /// <summary>
    /// محدوده سازمانی محاسبه‌شده BFF (چند مقداری).
    /// </summary>
    public const string OrganizationScope = "organization_scope";

    /// <summary>
    /// مرجع سازمان داخل محدوده BFF (چند مقداری؛ هر مقدار یک JSON شامل شناسه/نام/کد/والد/عمق).
    /// </summary>
    public const string OrganizationScopeNode = "organization_scope_node";
}