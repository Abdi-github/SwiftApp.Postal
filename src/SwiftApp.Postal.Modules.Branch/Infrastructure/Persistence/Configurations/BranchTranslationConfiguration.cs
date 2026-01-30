using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Branch.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Branch.Infrastructure.Persistence.Configurations;

public class BranchTranslationConfiguration : BaseTranslationConfiguration<BranchTranslation>
{
    protected override void ConfigureTranslation(EntityTypeBuilder<BranchTranslation> builder)
    {
        builder.ToTable("branch_translations");

        builder.HasQueryFilter(t => t.Branch.DeletedAt == null);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200).HasColumnName("name");
        builder.Property(e => e.Description).HasMaxLength(1000).HasColumnName("description");
        builder.Property(e => e.BranchId).HasColumnName("branch_id");

        builder.HasIndex(e => new { e.BranchId, e.Locale }).IsUnique();
    }
}
