using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>PostAuthorityAssignment</c>.
/// </summary>
public sealed class PostAuthorityAssignmentConfiguration
    : IEntityTypeConfiguration<PostAuthorityAssignment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PostAuthorityAssignment> builder)
    {
        builder.ToTable("PostAuthorityAssignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId).IsRequired();
        builder.Property(a => a.AuthorityId).IsRequired();
        builder.Property(a => a.OrganizationId).IsRequired();
        builder.Property(a => a.PostId).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();

        builder.HasOne<Domain.Entities.Post>()
            .WithMany()
            .HasForeignKey(a => a.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.TenantId, a.PostId });
        builder.HasIndex(a => new { a.TenantId, a.AuthorityId });

        builder.HasIndex(a => new { a.TenantId, a.PostId, a.AuthorityId })
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");
    }
}