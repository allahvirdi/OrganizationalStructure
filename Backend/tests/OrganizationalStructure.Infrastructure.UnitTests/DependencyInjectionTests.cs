using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationalStructure.Application.Integration.Iam;
using OrganizationalStructure.Infrastructure.Integration;
using StackExchange.Redis;

namespace OrganizationalStructure.Infrastructure.UnitTests;

/// <summary>
/// تست‌های ثبت مخزن نشست BFF در ظرف تزریق وابستگی
/// (انتخاب بین استور درون‌حافظه‌ای برای توسعه و استور Redis برای استقرار چندنمونه‌ای).
/// </summary>
public sealed class DependencyInjectionTests
{
    /// <summary>
    /// ساخت پیکربندی تست با رشته اتصال پایگاه داده و رشته اتصال اختیاری Redis.
    /// </summary>
    /// <param name="redisConnection">رشته اتصال Redis؛ نال یعنی بدون پیکربندی</param>
    private static IConfiguration BuildConfiguration(string? redisConnection)
    {
        var data = new Dictionary<string, string?>
        {
            ["ConnectionStrings:OrganizationalStructureDb"] = "Server=.;Database=OrgStructure;Trusted_Connection=True;"
        };

        if (redisConnection is not null)
        {
            data["ConnectionStrings:Redis"] = redisConnection;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }

    private static ServiceDescriptor? GetSessionStoreDescriptor(IServiceCollection services) =>
        services.FirstOrDefault(d => d.ServiceType == typeof(IBffSessionStore));

    /// <summary>
    /// وجود رشته اتصال Redis باید استور نشست مبتنی بر Redis را به‌صورت تک‌نمونه ثبت کند
    /// (پیش‌نیاز استقرار چندنمونه‌ای).
    /// </summary>
    [Fact]
    public void AddInfrastructure_WithRedisConnection_ShouldRegisterRedisSessionStore()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildConfiguration("localhost:6379"));

        var descriptor = GetSessionStoreDescriptor(services);
        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(RedisBffSessionStore));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        services.Should().Contain(d => d.ServiceType == typeof(IConnectionMultiplexer));
    }

    /// <summary>
    /// بدون پیکربندی Redis، استور درون‌حافظه‌ای باید ثبت شود (توسعه محلی و تست‌ها).
    /// </summary>
    [Fact]
    public void AddInfrastructure_WithoutRedisConnection_ShouldRegisterInMemorySessionStore()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildConfiguration(redisConnection: null));

        var descriptor = GetSessionStoreDescriptor(services);
        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(InMemoryBffSessionStore));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        services.Should().NotContain(d => d.ServiceType == typeof(IConnectionMultiplexer));
    }

    /// <summary>
    /// رشته اتصال Redis خالی یا فقط-فاصله باید مانند «بدون Redis» رفتار کند (استور درون‌حافظه‌ای).
    /// </summary>
    [Fact]
    public void AddInfrastructure_WithBlankRedisConnection_ShouldRegisterInMemorySessionStore()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildConfiguration("   "));

        var descriptor = GetSessionStoreDescriptor(services);
        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(InMemoryBffSessionStore));
        services.Should().NotContain(d => d.ServiceType == typeof(IConnectionMultiplexer));
    }
}