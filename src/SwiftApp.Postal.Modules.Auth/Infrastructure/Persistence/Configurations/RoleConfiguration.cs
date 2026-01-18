using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : BaseEntityConfiguration<Role>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100).HasColumnName("name");
        builder.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");

        builder.HasMany(e => e.Permissions)
            .WithMany(e => e.Roles)
            .UsingEntity("role_permissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("permission_id"),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("role_id"));

        builder.HasIndex(e => e.Name).IsUnique();
    }
}
