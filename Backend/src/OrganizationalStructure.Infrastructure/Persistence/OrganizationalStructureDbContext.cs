using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrganizationalStructure.Application.Common.Interfaces;
using OrganizationalStructure.Domain.Abstractions;
using OrganizationalStructure.Domain.Common;
using OrganizationalStructure.Domain.Encryption;
using OrganizationalStructure.Domain.Entities;

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
public sealed class OrganizationalStructureDbContext : DbContext, IAppDbContext
{
    private readonly ITenantContext _tenantContext;
    private readonly IPiiProtector _piiProtector;

    /// <inheritdoc />
    public DbSet<Post> Posts => Set<Post>();

    /// <inheritdoc />
    public DbSet<Employee> Employees => Set<Employee>();

    /// <inheritdoc />
    public DbSet<EmployeePostAssignment> Assignments => Set<EmployeePostAssignment>();

    /// <inheritdoc />
    public DbSet<Responsibility> Responsibilities => Set<Responsibility>();

    /// <inheritdoc />
    public DbSet<PostResponsibilityAssignment> ResponsibilityAssignments =>
        Set<PostResponsibilityAssignment>();

    /// <inheritdoc />
    public DbSet<Authority> Authorities => Set<Authority>();

    /// <inheritdoc />
    public DbSet<PostAuthorityAssignment> AuthorityAssignments =>
        Set<PostAuthorityAssignment>();

    /// <inheritdoc />
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();

    /// <inheritdoc />
    public DbSet<EmployeeStagingRow> EmployeeStagingRows => Set<EmployeeStagingRow>();

    /// <inheritdoc />
    public DbSet<ImportError> ImportErrors => Set<ImportError>();

    /// <summary>
    /// ساخت نمونه‌ی DbContext.
    /// </summary>
    /// <param name="options">تنظیمات پایگاه داده</param>
    /// <param name="tenantContext">متن مستأجر جاری برای Global Query Filter</param>
    /// <param name="piiProtector">محافظ داده‌های حساس برای تبدیل ستون‌های PII</param>
    public OrganizationalStructureDbContext(
        DbContextOptions<OrganizationalStructureDbContext> options,
        ITenantContext tenantContext,
        IPiiProtector piiProtector)
        : base(options)
    {
        _tenantContext = tenantContext;
        _piiProtector = piiProtector;
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

        ApplyPiiConversions(modelBuilder);

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

    /// <summary>
    /// اعمال تبدیل رمزنگاری روی ستون‌های PII پرسنل (ADR-006).
    /// </summary>
    /// <param name="modelBuilder">سازنده مدل</param>
    private void ApplyPiiConversions(ModelBuilder modelBuilder)
    {
        var randomizedText = new ValueConverter<string, string>(
            v => _piiProtector.Protect(v, EncryptionType.Randomized),
            v => _piiProtector.Unprotect(v));

        var deterministicText = new ValueConverter<string, string>(
            v => _piiProtector.Protect(v, EncryptionType.Deterministic),
            v => _piiProtector.Unprotect(v));

        var randomizedNullableText = new ValueConverter<string?, string?>(
            v => v == null ? null : _piiProtector.Protect(v, EncryptionType.Randomized),
            v => v == null ? null : _piiProtector.Unprotect(v));

        var deterministicNullableText = new ValueConverter<string?, string?>(
            v => v == null ? null : _piiProtector.Protect(v, EncryptionType.Deterministic),
            v => v == null ? null : _piiProtector.Unprotect(v));

        var birthDateConverter = new ValueConverter<DateOnly?, string?>(
            v => v.HasValue
                ? _piiProtector.Protect(v.Value.ToString("yyyy-MM-dd"), EncryptionType.Randomized)
                : null,
            v => v == null
                ? (DateOnly?)null
                : DateOnly.Parse(_piiProtector.Unprotect(v)));

        modelBuilder.Entity<Employee>().Property(e => e.FirstName).HasConversion(randomizedText);
        modelBuilder.Entity<Employee>().Property(e => e.LastName).HasConversion(randomizedText);
        modelBuilder.Entity<Employee>().Property(e => e.NationalCode).HasConversion(deterministicText);
        modelBuilder.Entity<Employee>().Property(e => e.Mobile).HasConversion(deterministicNullableText);
        modelBuilder.Entity<Employee>().Property(e => e.PezhvakMobile).HasConversion(deterministicNullableText);
        modelBuilder.Entity<Employee>().Property(e => e.BirthDate).HasConversion(birthDateConverter);

        // PII ردیف‌های واسط — هم‌راستا با Employees (DEC-030).
        modelBuilder.Entity<EmployeeStagingRow>().Property(r => r.FirstName).HasConversion(randomizedText);
        modelBuilder.Entity<EmployeeStagingRow>().Property(r => r.LastName).HasConversion(randomizedText);
        modelBuilder.Entity<EmployeeStagingRow>().Property(r => r.NationalCode).HasConversion(deterministicText);
        modelBuilder.Entity<EmployeeStagingRow>().Property(r => r.Mobile).HasConversion(deterministicNullableText);
        modelBuilder.Entity<EmployeeStagingRow>().Property(r => r.BirthDate).HasConversion(birthDateConverter);
    }
}