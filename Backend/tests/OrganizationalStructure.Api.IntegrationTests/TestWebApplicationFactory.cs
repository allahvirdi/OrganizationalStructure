using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Persistence;

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
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:OrganizationalStructureDb"] = ConnectionString
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<ICurrentUser, TestCurrentUser>();
        });
    }
}