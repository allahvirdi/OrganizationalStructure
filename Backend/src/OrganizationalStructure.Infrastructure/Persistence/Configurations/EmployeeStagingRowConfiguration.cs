using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core جدول ردیف‌های واسط پرسنل (DEC-030).
/// </summary>
/// <remarks>
/// بدون TenantId، بدون PezhvakMobile، بدون Soft Delete.
/// ستون‌های PII در DbContext با ValueConverter رمزنگاری می‌شوند.
/// </remarks>
public sealed class EmployeeStagingRowConfiguration : IEntityTypeConfiguration<EmployeeStagingRow>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EmployeeStagingRow> builder)
    {
        builder.ToTable("EmployeeStagingRows");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.BatchId).IsRequired();
        builder.Property(r => r.RowNumber).IsRequired();

        builder.Property(r => r.PersonnelCode)
            .IsRequired()
            .HasMaxLength(8)
            .IsFixedLength();

        // PII: maxLength 512 (رمزنگاری‌شده) — هم‌راستا با Employees.
        builder.Property(r => r.FirstName).IsRequired().HasMaxLength(512);
        builder.Property(r => r.LastName).IsRequired().HasMaxLength(512);
        builder.Property(r => r.NationalCode).IsRequired().HasMaxLength(512);
        builder.Property(r => r.Mobile).HasMaxLength(512);

        builder.Property(r => r.ServiceYears);
        builder.Property(r => r.ServiceMonths);

        builder.Property(r => r.ValidationStatus).IsRequired();
        builder.Property(r => r.EmployeeId);
        builder.Property(r => r.CommitStatus).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        // ایندکس.
        builder.HasIndex(r => r.BatchId);
    }
}