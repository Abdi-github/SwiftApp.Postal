using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class NotificationTemplateConfiguration : BaseEntityConfiguration<NotificationTemplate>
{
    protected override void ConfigureEntity(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");

        builder.Property(e => e.TemplateCode).IsRequired().HasMaxLength(100).HasColumnName("template_code");
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(50).HasColumnName("type");
        builder.Property(e => e.EventType).HasMaxLength(100).HasColumnName("event_type");

        builder.HasMany(e => e.Translations)
            .WithOne(t => t.NotificationTemplate)
            .HasForeignKey(t => t.NotificationTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.TemplateCode).IsUnique();
    }
}
