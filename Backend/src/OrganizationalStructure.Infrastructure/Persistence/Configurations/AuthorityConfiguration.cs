using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>Authority</c>.
/// </summary>
public sealed class AuthorityConfiguration : IEntityTypeConfiguration<Authority>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Authority> builder)
    {
        builder.ToTable("Authorities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId).IsRequired();

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description).HasMaxLength(1000);

        builder.Property(a => a.IsActive).IsRequired();

        builder.Property(a => a.Version).IsRowVersion();

        builder.HasIndex(a => new { a.TenantId, a.Code })
            .IsUnique();

        builder.HasMany(a => a.Assignments)
            .WithOne()
            .HasForeignKey(x => x.AuthorityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}