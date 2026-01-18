using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : BaseEntityConfiguration<Employee>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(20).HasColumnName("employee_number");
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100).HasColumnName("first_name");
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100).HasColumnName("last_name");
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256).HasColumnName("email");
        builder.Property(e => e.Phone).HasMaxLength(30).HasColumnName("phone");
        builder.Property(e => e.Role).HasConversion<string>().HasMaxLength(50).HasColumnName("role");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.AssignedBranchId).HasColumnName("assigned_branch_id");
        builder.Property(e => e.HireDate).HasColumnName("hire_date");
        builder.Property(e => e.PreferredLocale).HasMaxLength(10).HasColumnName("preferred_locale");
        builder.Property(e => e.KeycloakUserId).HasMaxLength(256).HasColumnName("keycloak_user_id");

        builder.HasIndex(e => e.EmployeeNumber).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.KeycloakUserId).IsUnique().HasFilter("keycloak_user_id IS NOT NULL");
    }
}
