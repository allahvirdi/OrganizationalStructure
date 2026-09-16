using Microsoft.AspNetCore.Authorization;

namespace OrganizationalStructure.API.Security;

/// <summary>
/// نام Policyهای تصریح‌دهی (یک Policy به‌ازای هر Permission مصوب DEC-025).
/// </summary>
/// <remarks>
/// هر Policy، Claim تستی `permission` با مقدار Permission متناظر را طلب می‌کند.
/// Fallback Policy، کاربر احراز هویت‌شده را برای همه Endpointها الزامی می‌کند (Deny by Default).
/// </remarks>
public static class AuthorizationPolicies
{
    /// <summary>
    /// نوع Claim دسترسی.
    /// </summary>
    public const string PermissionClaimType = "permission";

    /// <summary>
    /// Policyهای دامنه پست.
    /// </summary>
    public static class Post
    {
        /// <summary>مشاهده پست.</summary>
        public const string View = "OrganizationStructure.Post.View";
        /// <summary>ایجاد پست.</summary>
        public const string Create = "OrganizationStructure.Post.Create";
        /// <summary>ویرایش پست.</summary>
        public const string Update = "OrganizationStructure.Post.Update";
        /// <summary>فعال/غیرفعال پست.</summary>
        public const string Disable = "OrganizationStructure.Post.Disable";
        /// <summary>انتساب پرسنل به پست.</summary>
        public const string AssignEmployee = "OrganizationStructure.Post.AssignEmployee";
        /// <summary>قطع انتساب پرسنل از پست.</summary>
        public const string RemoveEmployee = "OrganizationStructure.Post.RemoveEmployee";
        /// <summary>مشاهده سلسله‌مراتب.</summary>
        public const string ViewHierarchy = "OrganizationStructure.Post.ViewHierarchy";
    }

    /// <summary>
    /// Policyهای دامنه پرسنل.
    /// </summary>
    public static class Employee
    {
        /// <summary>مشاهده پرسنل.</summary>
        public const string View = "OrganizationStructure.Employee.View";
        /// <summary>ثبت پرسنل.</summary>
        public const string Create = "OrganizationStructure.Employee.Create";
        /// <summary>ویرایش پرسنل.</summary>
        public const string Update = "OrganizationStructure.Employee.Update";
        /// <summary>فعال/غیرفعال پرسنل.</summary>
        public const string Disable = "OrganizationStructure.Employee.Disable";
        /// <summary>انتساب پست به پرسنل.</summary>
        public const string AssignPost = "OrganizationStructure.Employee.AssignPost";
        /// <summary>قطع انتساب پست از پرسنل.</summary>
        public const string RemovePost = "OrganizationStructure.Employee.RemovePost";
        /// <summary>ورود اطلاعات پرسنلی.</summary>
        public const string Import = "OrganizationStructure.Employee.Import";
        /// <summary>مشاهده داده حساس.</summary>
        public const string ViewSensitiveData = "OrganizationStructure.Employee.ViewSensitiveData";
    }

    /// <summary>
    /// Policyهای دامنه مسئولیت.
    /// </summary>
    public static class Responsibility
    {
        /// <summary>مشاهده مسئولیت.</summary>
        public const string View = "OrganizationStructure.Responsibility.View";
        /// <summary>تعریف مسئولیت.</summary>
        public const string Create = "OrganizationStructure.Responsibility.Create";
        /// <summary>ویرایش مسئولیت.</summary>
        public const string Update = "OrganizationStructure.Responsibility.Update";
        /// <summary>غیرفعال مسئولیت.</summary>
        public const string Disable = "OrganizationStructure.Responsibility.Disable";
        /// <summary>انتساب مسئولیت.</summary>
        public const string Assign = "OrganizationStructure.Responsibility.Assign";
        /// <summary>پایان انتساب مسئولیت.</summary>
        public const string EndAssignment = "OrganizationStructure.Responsibility.EndAssignment";
    }

    /// <summary>
    /// Policyهای دامنه اختیار.
    /// </summary>
    public static class Authority
    {
        /// <summary>مشاهده اختیار.</summary>
        public const string View = "OrganizationStructure.Authority.View";
        /// <summary>تعریف اختیار.</summary>
        public const string Create = "OrganizationStructure.Authority.Create";
        /// <summary>ویرایش اختیار.</summary>
        public const string Update = "OrganizationStructure.Authority.Update";
        /// <summary>غیرفعال اختیار.</summary>
        public const string Disable = "OrganizationStructure.Authority.Disable";
        /// <summary>انتساب اختیار.</summary>
        public const string Assign = "OrganizationStructure.Authority.Assign";
        /// <summary>پایان انتساب اختیار.</summary>
        public const string EndAssignment = "OrganizationStructure.Authority.EndAssignment";
    }

    /// <summary>
    /// همه Policyها برای ثبت گروهی.
    /// </summary>
    public static IReadOnlyList<string> All { get; } = new[]
    {
        Post.View, Post.Create, Post.Update, Post.Disable,
        Post.AssignEmployee, Post.RemoveEmployee, Post.ViewHierarchy,
        Employee.View, Employee.Create, Employee.Update, Employee.Disable,
        Employee.AssignPost, Employee.RemovePost, Employee.Import, Employee.ViewSensitiveData,
        Responsibility.View, Responsibility.Create, Responsibility.Update, Responsibility.Disable,
        Responsibility.Assign, Responsibility.EndAssignment,
        Authority.View, Authority.Create, Authority.Update, Authority.Disable,
        Authority.Assign, Authority.EndAssignment
    };
}

/// <summary>
/// ثبت Policyهای تصریح‌دهی.
/// </summary>
public static class AuthorizationPoliciesExtensions
{
    /// <summary>
    /// ثبت ۲۷ Policy دامنه + Fallback Deny by Default.
    /// </summary>
    /// <param name="services">مجموعه خدمات</param>
    /// <returns>مجموعه خدمات</returns>
    public static IServiceCollection AddOrgAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var policy in AuthorizationPolicies.All)
            {
                options.AddPolicy(
                    policy,
                    policyBuilder => policyBuilder.RequireClaim(
                        AuthorizationPolicies.PermissionClaimType,
                        policy));
            }

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}