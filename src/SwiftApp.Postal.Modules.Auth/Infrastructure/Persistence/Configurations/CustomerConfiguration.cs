using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : BaseEntityConfiguration<Customer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.Property(e => e.CustomerNumber).IsRequired().HasMaxLength(20).HasColumnName("customer_number");
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100).HasColumnName("first_name");
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100).HasColumnName("last_name");
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256).HasColumnName("email");
        builder.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.PreferredLocale).HasMaxLength(10).HasColumnName("preferred_locale");
        builder.Property(e => e.KeycloakUserId).HasMaxLength(256).HasColumnName("keycloak_user_id");

        builder.HasIndex(e => e.CustomerNumber).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.KeycloakUserId).IsUnique().HasFilter("keycloak_user_id IS NOT NULL");
    }
}
