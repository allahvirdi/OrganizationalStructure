using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Application.Behaviors;

namespace OrganizationalStructure.Application;

/// <summary>
/// نقطه ثبت خدمات لایه کاربرد در ظرف تزریق وابستگی.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// ثبت خدمات لایه کاربرد (MediatR، اعتبارسنجی، نگاشت).
    /// </summary>
    /// <param name="services">مجموعه خدمات</param>
    /// <returns>مجموعه خدمات برای زنجیره‌سازی</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMapster();

        return services;
    }
}