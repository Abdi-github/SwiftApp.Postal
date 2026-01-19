using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Configurations;

public class DeliveryAttemptConfiguration : BaseEntityConfiguration<DeliveryAttempt>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DeliveryAttempt> builder)
    {
        builder.ToTable("delivery_attempts");

        builder.Property(e => e.DeliverySlotId).HasColumnName("delivery_slot_id");
        builder.Property(e => e.Result).HasConversion<string>().HasMaxLength(50).HasColumnName("result");
        builder.Property(e => e.Notes).HasMaxLength(500).HasColumnName("notes");
        builder.Property(e => e.AttemptTimestamp).HasColumnName("attempt_timestamp");
    }
}
