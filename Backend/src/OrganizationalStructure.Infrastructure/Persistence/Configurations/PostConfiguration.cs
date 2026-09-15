using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>Post</c> (مطابق ERD منطقی).
/// </summary>
public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TenantId).IsRequired();
        builder.Property(p => p.OrganizationId).IsRequired();

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description).HasMaxLength(1000);

        builder.HasOne(p => p.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.IsActive).IsRequired();

        builder.Property(p => p.Version).IsRowVersion();

        builder.HasIndex(p => new { p.TenantId, p.OrganizationId, p.Code })
            .IsUnique();

        builder.HasIndex(p => new { p.TenantId, p.ParentId });
    }
}