using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Tracking.Infrastructure.Persistence.Configurations;

public class TrackingEventConfiguration : BaseEntityConfiguration<TrackingEvent>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TrackingEvent> builder)
    {
        builder.ToTable("tracking_events");

        builder.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(20).HasColumnName("tracking_number");
        builder.Property(e => e.EventType).HasConversion<string>().HasMaxLength(50).HasColumnName("event_type");
        builder.Property(e => e.BranchId).HasColumnName("branch_id");
        builder.Property(e => e.Location).HasMaxLength(200).HasColumnName("location");
        builder.Property(e => e.DescriptionKey).HasMaxLength(200).HasColumnName("description_key");
        builder.Property(e => e.EventTimestamp).HasColumnName("event_timestamp");
        builder.Property(e => e.ScannedByEmployeeId).HasMaxLength(100).HasColumnName("scanned_by_employee_id");

        builder.HasIndex(e => e.TrackingNumber);
        builder.HasIndex(e => e.EventTimestamp);
    }
}
