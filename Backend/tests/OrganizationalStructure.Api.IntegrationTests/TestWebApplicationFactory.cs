using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Persistence;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Api.IntegrationTests;

/// <summary>
/// کارخانه اپلیکیشن تست با پایگاه داده ایزوله LocalDB و اعمال Migration.
/// </summary>
/// <remarks>
/// هر نمونه، یک دیتابیس یکتا روی LocalDB می‌سازد و Migration اولیه را اعمال می‌کند.
/// پیش‌نیاز اجرا: LocalDB (MSSQLLocalDB) در دسترس باشد.
/// </remarks>
public sealed class TestWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private string _databaseName = string.Empty;

    /// <summary>
    /// رشته اتصال دیتابیس تست جاری.
    /// </summary>
    public string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True";

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _databaseName = $"OrgStructure_Test_{Guid.NewGuid():N}";

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrganizationalStructureDbContext>();
        await db.Database.MigrateAsync();
    }

    /// <inheritdoc />
    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrganizationalStructureDbContext>();
        await db.Database.EnsureDeletedAsync();

        await base.DisposeAsync();
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, null);

            // محافظ PII تست‌-only با کلید ثابت (هرگز در Production استفاده نمی‌شود)
            services.AddSingleton<IPiiProtector>(_ => new AesPiiProtector(
                Microsoft.Extensions.Options.Options.Create(new PiiEncryptionOptions
                {
                    Key = TestKeys.Pii
                })));

            // بازنویسی مستقیم اتصال DbContext به دیتابیس ایزوله تست.
            // (override پیکربندی به‌تنهایی برای رشته اتصال اعمال نشد؛ این روش قطعی است)
            var optionsDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<OrganizationalStructureDbContext>));
            if (optionsDescriptor is not null)
            {
                services.Remove(optionsDescriptor);
            }

            var contextDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(OrganizationalStructureDbContext));
            if (contextDescriptor is not null)
            {
                services.Remove(contextDescriptor);
            }

            services.AddDbContext<OrganizationalStructureDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    ConnectionString,
                    sql => sql.MigrationsAssembly(
                        typeof(OrganizationalStructureDbContext).Assembly.FullName));
                options.AddInterceptors(new AuditSaveChangesInterceptor(
                    sp.GetRequiredService<IClock>(),
                    sp.GetRequiredService<ICurrentUser>()));
            });
        });
    }
}