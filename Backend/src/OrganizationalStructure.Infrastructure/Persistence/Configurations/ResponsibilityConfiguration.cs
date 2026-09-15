using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>Responsibility</c>.
/// </summary>
public sealed class ResponsibilityConfiguration : IEntityTypeConfiguration<Responsibility>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Responsibility> builder)
    {
        builder.ToTable("Responsibilities");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId).IsRequired();

        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description).HasMaxLength(1000);

        builder.Property(r => r.IsActive).IsRequired();

        builder.Property(r => r.Version).IsRowVersion();

        builder.HasIndex(r => new { r.TenantId, r.Code })
            .IsUnique();

        builder.HasMany(r => r.Assignments)
            .WithOne()
            .HasForeignKey(a => a.ResponsibilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}