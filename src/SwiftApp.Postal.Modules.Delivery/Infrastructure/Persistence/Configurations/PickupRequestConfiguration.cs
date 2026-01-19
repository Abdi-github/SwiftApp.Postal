using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Configurations;

public class PickupRequestConfiguration : BaseEntityConfiguration<PickupRequest>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PickupRequest> builder)
    {
        builder.ToTable("pickup_requests");

        builder.Property(e => e.CustomerId).HasColumnName("customer_id");
        builder.Property(e => e.PickupStreet).IsRequired().HasMaxLength(200).HasColumnName("pickup_street");
        builder.Property(e => e.PickupZipCode).IsRequired().HasMaxLength(10).HasColumnName("pickup_zip_code");
        builder.Property(e => e.PickupCity).IsRequired().HasMaxLength(100).HasColumnName("pickup_city");
        builder.Property(e => e.PreferredDate).HasColumnName("preferred_date");
        builder.Property(e => e.PreferredTimeFrom).HasColumnName("preferred_time_from");
        builder.Property(e => e.PreferredTimeTo).HasColumnName("preferred_time_to");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
    }
}
