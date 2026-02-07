using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Tracking.Infrastructure.Persistence.Configurations;

public class TrackingRecordConfiguration : BaseEntityConfiguration<TrackingRecord>
{
    protected override void ConfigureEntity(EntityTypeBuilder<TrackingRecord> builder)
    {
        builder.ToTable("tracking_records");

        builder.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(20).HasColumnName("tracking_number");
        builder.Property(e => e.CurrentStatus).IsRequired().HasMaxLength(50).HasColumnName("current_status");
        builder.Property(e => e.CurrentBranchId).HasColumnName("current_branch_id");
        builder.Property(e => e.EstimatedDelivery).HasColumnName("estimated_delivery");

        builder.HasIndex(e => e.TrackingNumber).IsUnique();
    }
}
