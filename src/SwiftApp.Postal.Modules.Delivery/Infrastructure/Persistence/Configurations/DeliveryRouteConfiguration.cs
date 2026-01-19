using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Configurations;

public class DeliveryRouteConfiguration : BaseEntityConfiguration<DeliveryRoute>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DeliveryRoute> builder)
    {
        builder.ToTable("delivery_routes");

        builder.Property(e => e.RouteCode).IsRequired().HasMaxLength(20).HasColumnName("route_code");
        builder.Property(e => e.BranchId).HasColumnName("branch_id");
        builder.Property(e => e.AssignedEmployeeId).HasColumnName("assigned_employee_id");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.Date).HasColumnName("date");

        builder.HasMany(e => e.Slots)
            .WithOne(s => s.DeliveryRoute)
            .HasForeignKey(s => s.DeliveryRouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.RouteCode).IsUnique();
    }
}
