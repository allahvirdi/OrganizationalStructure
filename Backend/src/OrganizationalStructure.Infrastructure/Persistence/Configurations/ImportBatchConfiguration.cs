using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core جدول بارگذاری‌های واسط (DEC-030).
/// </summary>
/// <remarks>
/// بدون TenantId و بدون Soft Delete — جدول موقتی با حذف فیزیکی پس از ۱۰ روز.
/// </remarks>
public sealed class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ImportBatch> builder)
    {
        builder.ToTable("ImportBatches");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.OrganizationId).IsRequired();

        builder.Property(b => b.Source).IsRequired();

        builder.Property(b => b.FileName).HasMaxLength(255);

        builder.Property(b => b.Status).IsRequired();

        builder.Property(b => b.TotalRows).IsRequired().HasDefaultValue(0);
        builder.Property(b => b.ValidRows).IsRequired().HasDefaultValue(0);
        builder.Property(b => b.InvalidRows).IsRequired().HasDefaultValue(0);

        builder.Property(b => b.CommittedCount);

        builder.Property(b => b.Notes).HasMaxLength(500);

        builder.Property(b => b.ReviewedById);
        builder.Property(b => b.ReviewedAt);
        builder.Property(b => b.CommittedAt);

        builder.Property(b => b.Version).IsRowVersion();

        // ایندکس‌ها مطابق سند طراحی.
        builder.HasIndex(b => new { b.OrganizationId, b.Status });
        builder.HasIndex(b => b.CreatedAt).IsDescending();

        // روابط.
        builder.HasMany<EmployeeStagingRow>()
            .WithOne()
            .HasForeignKey(r => r.BatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<ImportError>()
            .WithOne()
            .HasForeignKey(e => e.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}