using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Common;
using OrganizationalStructure.Infrastructure.Integration;
using OrganizationalStructure.Infrastructure.Persistence;
using OrganizationalStructure.Infrastructure.Security;
using StackExchange.Redis;

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

        services.Configure<PiiEncryptionOptions>(
            configuration.GetSection(PiiEncryptionOptions.SectionName));
        services.AddSingleton<IPiiProtector, AesPiiProtector>();

        services.Configure<IamOptions>(configuration.GetSection(IamOptions.SectionName));
        services.AddHttpClient("iam");
        services.AddScoped<IIamClient, IamClient>();

        // نشست BFF باید Singleton باشد؛ با AddScoped هر درخواست استور خالی می‌گرفت (علت 401 نشست).
        // اگر ConnectionStrings:Redis مقدار داشته باشد → نشست در Redis (استقرار چندنمونه‌ای / Production)؛
        // در غیر این صورت → حافظه داخلی (توسعه محلی و تست‌ها).
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(redisConnection));
            services.AddSingleton<Application.Integration.Iam.IBffSessionStore, Integration.RedisBffSessionStore>();
        }
        else
        {
            services.AddSingleton<Application.Integration.Iam.IBffSessionStore, Integration.InMemoryBffSessionStore>();
        }

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

        services.AddScoped<Application.Common.Interfaces.IAppDbContext>(
            sp => sp.GetRequiredService<OrganizationalStructureDbContext>());

        services.AddHealthChecks()
            .AddDbContextCheck<OrganizationalStructureDbContext>(
                name: "organizational-structure-db",
                tags: new[] { "db", "sqlserver" });

        return services;
    }
}