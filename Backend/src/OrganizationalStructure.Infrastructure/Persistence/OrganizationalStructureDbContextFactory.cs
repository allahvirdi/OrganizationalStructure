using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Infrastructure.Persistence;

/// <summary>
/// کارخانه ساخت DbContext در زمان طراحی برای ابزار EF Core (Migration).
/// </summary>
/// <remarks>
/// رشته اتصال و کلید رمزنگاری PII از پیکربندی (appsettings / appsettings.{Environment} /
/// متغیرهای محیطی / User Secrets) خوانده می‌شود تا هیچ رمزی در کد یا مخزن ثبت نشود.
/// در زمان طراحی، ICurrentUser در دسترس نیست؛ بنابراین DbContext بدون interceptor ساخته می‌شود.
/// </remarks>
public sealed class OrganizationalStructureDbContextFactory
    : IDesignTimeDbContextFactory<OrganizationalStructureDbContext>
{
    /// <summary>
    /// نمونه DbContext را با اتصال خوانده‌شده از پیکربندی می‌سازد.
    /// </summary>
    /// <param name="args">آرگومان‌های ابزار EF</param>
    /// <returns>DbContext آماده طراحی</returns>
    public OrganizationalStructureDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "OrganizationalStructure.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.Exists(apiPath) ? apiPath : Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("OrganizationalStructureDb")
            ?? throw new InvalidOperationException("رشته اتصال OrganizationalStructureDb در پیکربندی یافت نشد.");

        var piiOptions = new PiiEncryptionOptions();
        configuration.GetSection(PiiEncryptionOptions.SectionName).Bind(piiOptions);
        IPiiProtector protector = new AesPiiProtector(Options.Create(piiOptions));

        var options = new DbContextOptionsBuilder<OrganizationalStructureDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new OrganizationalStructureDbContext(options, new DesignTimeTenantContext(), protector);
    }

    /// <summary>
    /// متن مستأجر موقت برای زمان طراحی (فقط ساخت مدل، بدون اجرای کوئری).
    /// </summary>
    private sealed class DesignTimeTenantContext : ITenantContext
    {
        /// <summary>
        /// شناسه مستأجر خالی در زمان طراحی.
        /// </summary>
        public Guid TenantId => Guid.Empty;
    }
}