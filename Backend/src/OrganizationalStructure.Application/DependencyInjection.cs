using Microsoft.Extensions.DependencyInjection;

namespace OrganizationalStructure.Application;

/// <summary>
/// نقطه ثبت خدمات لایه کاربرد در ظرف تزریق وابستگی.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// ثبت خدمات لایه کاربرد.
    /// </summary>
    /// <param name="services">مجموعه خدمات</param>
    /// <returns>مجموعه خدمات برای زنجیره‌سازی</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Handlers، Validators و Behaviors لایه کاربرد در فاز 3 (Vertical Slices) در اینجا ثبت می‌شوند.
        return services;
    }
}