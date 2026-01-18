using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : BaseEntityConfiguration<Permission>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        builder.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");

        builder.HasIndex(e => e.Name).IsUnique();
    }
}
