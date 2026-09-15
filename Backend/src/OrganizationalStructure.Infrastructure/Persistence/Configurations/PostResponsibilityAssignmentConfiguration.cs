using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>PostResponsibilityAssignment</c>.
/// </summary>
public sealed class PostResponsibilityAssignmentConfiguration
    : IEntityTypeConfiguration<PostResponsibilityAssignment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PostResponsibilityAssignment> builder)
    {
        builder.ToTable("PostResponsibilityAssignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId).IsRequired();
        builder.Property(a => a.ResponsibilityId).IsRequired();
        builder.Property(a => a.OrganizationId).IsRequired();
        builder.Property(a => a.PostId).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();

        builder.HasOne<Domain.Entities.Post>()
            .WithMany()
            .HasForeignKey(a => a.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.TenantId, a.PostId });
        builder.HasIndex(a => new { a.TenantId, a.ResponsibilityId });

        builder.HasIndex(a => new { a.TenantId, a.PostId, a.ResponsibilityId })
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");
    }
}