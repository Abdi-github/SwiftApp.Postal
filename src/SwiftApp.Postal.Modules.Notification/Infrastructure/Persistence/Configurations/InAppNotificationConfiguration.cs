using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class InAppNotificationConfiguration : BaseEntityConfiguration<InAppNotification>
{
    protected override void ConfigureEntity(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("in_app_notifications");

        builder.Property(e => e.TargetEmployeeId).HasColumnName("target_employee_id");
        builder.Property(e => e.TargetRole).HasMaxLength(50).HasColumnName("target_role");
        builder.Property(e => e.TargetBranchId).HasColumnName("target_branch_id");
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200).HasColumnName("title");
        builder.Property(e => e.Message).IsRequired().HasMaxLength(2000).HasColumnName("message");
        builder.Property(e => e.Category).HasConversion<string>().HasMaxLength(50).HasColumnName("category");
        builder.Property(e => e.ReferenceUrl).HasMaxLength(500).HasColumnName("reference_url");
        builder.Property(e => e.SenderEmployeeId).HasColumnName("sender_employee_id");

        builder.HasMany(e => e.ReadReceipts)
            .WithOne(r => r.InAppNotification)
            .HasForeignKey(r => r.InAppNotificationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
