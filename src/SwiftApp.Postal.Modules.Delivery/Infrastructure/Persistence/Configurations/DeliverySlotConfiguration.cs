using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Configurations;

public class DeliverySlotConfiguration : BaseEntityConfiguration<DeliverySlot>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DeliverySlot> builder)
    {
        builder.ToTable("delivery_slots");

        builder.Property(e => e.DeliveryRouteId).HasColumnName("delivery_route_id");
        builder.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(20).HasColumnName("tracking_number");
        builder.Property(e => e.SequenceOrder).HasColumnName("sequence_order");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.RecipientSignature).HasMaxLength(500).HasColumnName("recipient_signature");

        builder.HasMany(e => e.Attempts)
            .WithOne(a => a.DeliverySlot)
            .HasForeignKey(a => a.DeliverySlotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
