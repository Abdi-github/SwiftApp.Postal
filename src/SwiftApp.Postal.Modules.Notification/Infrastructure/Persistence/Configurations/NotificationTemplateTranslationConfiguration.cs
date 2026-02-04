using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class NotificationTemplateTranslationConfiguration : BaseTranslationConfiguration<NotificationTemplateTranslation>
{
    protected override void ConfigureTranslation(EntityTypeBuilder<NotificationTemplateTranslation> builder)
    {
        builder.ToTable("notification_template_translations");

        builder.HasQueryFilter(t => t.NotificationTemplate.DeletedAt == null);

        builder.Property(e => e.NotificationTemplateId).HasColumnName("notification_template_id");
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(500).HasColumnName("subject");
        builder.Property(e => e.Body).IsRequired().HasColumnName("body");

        builder.HasIndex(e => new { e.NotificationTemplateId, e.Locale }).IsUnique();
    }
}
