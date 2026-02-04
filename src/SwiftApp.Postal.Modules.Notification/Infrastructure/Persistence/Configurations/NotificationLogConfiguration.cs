using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : BaseEntityConfiguration<NotificationLog>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("notification_logs");

        builder.Property(e => e.RecipientEmail).HasMaxLength(256).HasColumnName("recipient_email");
        builder.Property(e => e.RecipientPhone).HasMaxLength(30).HasColumnName("recipient_phone");
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50).HasColumnName("type");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasColumnName("status");
        builder.Property(e => e.Subject).HasMaxLength(500).HasColumnName("subject");
        builder.Property(e => e.Body).HasColumnName("body");
        builder.Property(e => e.ReferenceId).HasMaxLength(100).HasColumnName("reference_id");
        builder.Property(e => e.EventType).HasMaxLength(100).HasColumnName("event_type");
        builder.Property(e => e.RetryCount).HasColumnName("retry_count");
    }
}
