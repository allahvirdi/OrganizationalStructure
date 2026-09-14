using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Common;
using OrganizationalStructure.Infrastructure.Persistence;

namespace OrganizationalStructure.Infrastructure;

/// <summary>
/// نقطه ثبت خدمات لایه زیرساخت در ظرف تزریق وابستگی.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// ثبت خدمات لایه زیرساخت.
    /// </summary>
    /// <param name="services">مجموعه خدمات</param>
    /// <param name="configuration">پیکربندی</param>
    /// <returns>مجموعه خدمات برای زنجیره‌سازی</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrganizationalStructureDb")
            ?? throw new InvalidOperationException(
                "Connection string 'OrganizationalStructureDb' is not configured.");

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ITenantContext, CurrentUserTenantContext>();

        // ICurrentUser به وسیله لایه API (پیاده‌سازی BFF) ثبت می‌شود.
        // DbContext و interceptor به صورت Scoped و مبتنی بر ICurrentUser/ITenantContext ثبت می‌شوند.
        services.AddDbContext<OrganizationalStructureDbContext>((sp, options) =>
        {
            var clock = sp.GetRequiredService<IClock>();
            var currentUser = sp.GetRequiredService<ICurrentUser>();

            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(OrganizationalStructureDbContext).Assembly.FullName));
            options.AddInterceptors(new AuditSaveChangesInterceptor(clock, currentUser));
        });

        services.AddHealthChecks()
            .AddDbContextCheck<OrganizationalStructureDbContext>(
                name: "organizational-structure-db",
                tags: new[] { "db", "sqlserver" });

        return services;
    }
}