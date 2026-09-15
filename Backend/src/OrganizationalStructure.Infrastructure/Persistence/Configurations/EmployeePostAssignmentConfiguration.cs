using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizationalStructure.Domain.Entities;

namespace OrganizationalStructure.Infrastructure.Persistence.Configurations;

/// <summary>
/// پیکربندی EF Core موجودیت <c>EmployeePostAssignment</c> (مطابق ERD منطقی).
/// </summary>
public sealed class EmployeePostAssignmentConfiguration : IEntityTypeConfiguration<EmployeePostAssignment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<EmployeePostAssignment> builder)
    {
        builder.ToTable("EmployeePostAssignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId).IsRequired();
        builder.Property(a => a.EmployeeId).IsRequired();
        builder.Property(a => a.PostId).IsRequired();
        builder.Property(a => a.IsPrimary).IsRequired();

        builder.HasOne<Domain.Entities.Post>()
            .WithMany()
            .HasForeignKey(a => a.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.TenantId, a.EmployeeId });
        builder.HasIndex(a => new { a.TenantId, a.PostId });

        builder.HasIndex(a => new { a.TenantId, a.EmployeeId, a.PostId })
            .IsUnique()
            .HasFilter("[ToDate] IS NULL AND [IsDeleted] = 0");
    }
}