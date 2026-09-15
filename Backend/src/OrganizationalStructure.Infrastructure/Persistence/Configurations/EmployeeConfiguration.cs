using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>Employee</c> (مطابق ERD منطقی).
/// </summary>
/// <remarks>
/// رمزنگاری ستون‌های PII (ADR-006) در Slice پرسنل (تسک بعدی فاز ۳) اعمال می‌شود؛
/// این پیکربندی فقط ساختار ستون‌ها را تعریف می‌کند.
/// </remarks>
public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TenantId).IsRequired();

        builder.Property(e => e.UserId);

        builder.Property(e => e.PersonnelCode)
            .IsRequired()
            .HasMaxLength(8)
            .IsFixedLength();

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.NationalCode).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Mobile).HasMaxLength(20);
        builder.Property(e => e.PezhvakMobile).HasMaxLength(20);

        builder.Property(e => e.IsActive).IsRequired();

        builder.Property(e => e.Version).IsRowVersion();

        builder.HasIndex(e => new { e.TenantId, e.PersonnelCode })
            .IsUnique();

        builder.HasIndex(e => new { e.TenantId, e.UserId });

        builder.OwnsOne(e => e.ServiceRecord, owned =>
        {
            owned.Property(s => s.Years).HasColumnName("ServiceYears");
            owned.Property(s => s.Months).HasColumnName("ServiceMonths");
        });

        builder.HasMany(e => e.Assignments)
            .WithOne()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}