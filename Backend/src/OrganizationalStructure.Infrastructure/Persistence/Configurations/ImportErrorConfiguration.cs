using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core جدول خطاهای بارگذاری واسط (DEC-030).
/// </summary>
/// <remarks>
/// پیام خطا فاقد مقدار حساس (ADR-006). حذف فیزیکی همراه با Batch.
/// </remarks>
public sealed class ImportErrorConfiguration : IEntityTypeConfiguration<ImportError>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ImportError> builder)
    {
        builder.ToTable("ImportErrors");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.BatchId).IsRequired();
        builder.Property(e => e.StagingRowId);
        builder.Property(e => e.RowNumber);
        builder.Property(e => e.ColumnName).HasMaxLength(100);
        builder.Property(e => e.ErrorCode).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Message).IsRequired().HasMaxLength(500);
        builder.Property(e => e.CreatedAt).IsRequired();

        // ایندکس.
        builder.HasIndex(e => e.BatchId);
    }
}