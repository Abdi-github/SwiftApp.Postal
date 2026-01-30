using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Branch.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Branch.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : BaseEntityConfiguration<Domain.Entities.Branch>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Domain.Entities.Branch> builder)
    {
        builder.ToTable("branches");

        builder.Property(e => e.BranchCode).IsRequired().HasMaxLength(20).HasColumnName("branch_code");
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50).HasColumnName("type");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.Street).IsRequired().HasMaxLength(200).HasColumnName("street");
        builder.Property(e => e.ZipCode).IsRequired().HasMaxLength(10).HasColumnName("zip_code");
        builder.Property(e => e.City).IsRequired().HasMaxLength(100).HasColumnName("city");
        builder.Property(e => e.Canton).HasMaxLength(5).HasColumnName("canton");
        builder.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
        builder.Property(e => e.Email).HasMaxLength(256).HasColumnName("email");
        builder.Property(e => e.Latitude).HasColumnType("NUMERIC(10,7)").HasColumnName("latitude");
        builder.Property(e => e.Longitude).HasColumnType("NUMERIC(10,7)").HasColumnName("longitude");

        builder.HasMany(e => e.Translations)
            .WithOne(t => t.Branch)
            .HasForeignKey(t => t.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.BranchCode).IsUnique();
    }
}
