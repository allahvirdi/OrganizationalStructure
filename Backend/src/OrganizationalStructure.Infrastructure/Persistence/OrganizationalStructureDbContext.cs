using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Common;

namespace OrganizationalStructure.Infrastructure.Persistence;

/// <summary>
/// DbContext اصلی سامانه.
/// </summary>
/// <remarks>
/// Global Query Filter برای جداسازی داده به‌ازای مستأجر و نادیده‌گرفتن رکوردهای Soft Delete شده
/// روی تمام موجودیت‌های مشتق از <see cref="TenantEntity"/> اعمال می‌شود (Multi-tenancy + ADR-005).
///
/// فیلتر مستأجر با ارجاع به <see cref="CurrentTenantId"/> بسته می‌شود؛ EF Core این ارجاع را در
/// <b>هر بار اجرای کوئری</b> ارزیابی می‌کند (نه هنگام ساخت مدل)، بنابراین مقدار مستأجر
/// به‌صورت داینامیک از متن مستأجر جاری خوانده می‌شود.
/// </remarks>
public sealed class OrganizationalStructureDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// ساخت نمونه‌ی DbContext.
    /// </summary>
    /// <param name="options">تنظیمات پایگاه داده</param>
    /// <param name="tenantContext">متن مستأجر جاری برای Global Query Filter</param>
    public OrganizationalStructureDbContext(
        DbContextOptions<OrganizationalStructureDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// شناسه مستأجر جاری؛ در هر بار اجرای کوئری توسط فیلتر سراسری ارزیابی می‌شود.
    /// </summary>
    public Guid CurrentTenantId => _tenantContext.TenantId;

    /// <summary>
    /// پیکربندی مدل و اعمال Global Query Filters.
    /// </summary>
    /// <param name="modelBuilder">سازنده مدل</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationalStructureDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var tenantProperty = Expression.Property(parameter, nameof(ITenantScoped.TenantId));
                var currentTenant = Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentTenantId));
                var body = Expression.Equal(tenantProperty, currentTenant);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    Expression.Lambda(body, parameter));
            }

            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(parameter, nameof(TenantEntity.IsDeleted)),
                    Expression.Constant(false));

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                    Expression.Lambda(body, parameter));
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}