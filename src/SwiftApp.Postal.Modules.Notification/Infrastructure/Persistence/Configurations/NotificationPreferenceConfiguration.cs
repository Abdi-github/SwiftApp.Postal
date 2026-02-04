using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class NotificationPreferenceConfiguration : BaseEntityConfiguration<NotificationPreference>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("notification_preferences");

        builder.Property(e => e.CustomerId).HasColumnName("customer_id");
        builder.Property(e => e.EmailEnabled).HasColumnName("email_enabled");
        builder.Property(e => e.SmsEnabled).HasColumnName("sms_enabled");
        builder.Property(e => e.InAppEnabled).HasColumnName("in_app_enabled");
        builder.Property(e => e.PreferredLocale).HasMaxLength(10).HasColumnName("preferred_locale");

        builder.HasIndex(e => e.CustomerId).IsUnique();
    }
}
