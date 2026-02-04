using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Address.Infrastructure.Persistence.Configurations;

public class SwissAddressConfiguration : BaseEntityConfiguration<SwissAddress>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SwissAddress> builder)
    {
        builder.ToTable("swiss_addresses");

        builder.Property(e => e.ZipCode).IsRequired().HasMaxLength(10).HasColumnName("zip_code");
        builder.Property(e => e.City).IsRequired().HasMaxLength(100).HasColumnName("city");
        builder.Property(e => e.Canton).IsRequired().HasMaxLength(5).HasColumnName("canton");
        builder.Property(e => e.Municipality).HasMaxLength(100).HasColumnName("municipality");

        builder.HasIndex(e => e.ZipCode);
        builder.HasIndex(e => e.Canton);
    }
}
