using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Infrastructure.Persistence;
using OrganizationalStructure.Infrastructure.Security;

namespace OrganizationalStructure.Application.UnitTests;

/// <summary>
/// دابل‌های تست (مجاز فقط در پروژه تست): ساعت ثابت، کاربر جاری ثابت و DbContext درون‌حافظه‌ای.
/// </summary>
public sealed class TestClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow { get; } =
        new(2026, 9, 14, 12, 0, 0, TimeSpan.Zero);
}

/// <summary>
/// کاربر جاری ثابت برای تست.
/// </summary>
public sealed class TestCurrentUser : ICurrentUser
{
    private readonly Guid _tenantId;

    /// <summary>
    /// ساخت کاربر جاری تست.
    /// </summary>
    /// <param name="tenantId">شناسه مستأجر ثابت</param>
    public TestCurrentUser(Guid tenantId)
    {
        _tenantId = tenantId;
    }

    /// <inheritdoc />
    public bool IsAuthenticated => true;

    /// <inheritdoc />
    public Guid UserId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public Guid TenantId => _tenantId;

    /// <inheritdoc />
    public Guid? OrganizationId => null;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles => Array.Empty<string>();
}

/// <summary>
/// متن مستأجر ثابت برای تست.
/// </summary>
public sealed class TestTenantContext : ITenantContext
{
    /// <summary>
    /// ساخت متن مستأجر تست.
    /// </summary>
    /// <param name="tenantId">شناسه مستأجر ثابت</param>
    public TestTenantContext(Guid tenantId)
    {
        TenantId = tenantId;
    }

    /// <inheritdoc />
    public Guid TenantId { get; }
}

/// <summary>
/// کارخانه ساخت DbContext درون‌حافظه‌ای ایزوله برای هر تست.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// ساخت DbContext جدید با پایگاه درون‌حافظه‌ای یکتا و محافظ PII واقعی (کلید تصادفی تست).
    /// </summary>
    /// <param name="tenantId">شناسه مستأجر ثابت</param>
    /// <returns>DbContext تست</returns>
    public static OrganizationalStructureDbContext Create(Guid? tenantId = null)
    {
        var tenant = tenantId ?? Guid.NewGuid();
        var options = new DbContextOptionsBuilder<OrganizationalStructureDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var protector = new AesPiiProtector(
            Options.Create(new PiiEncryptionOptions { Key = key }));

        return new OrganizationalStructureDbContext(options, new TestTenantContext(tenant), protector);
    }
}