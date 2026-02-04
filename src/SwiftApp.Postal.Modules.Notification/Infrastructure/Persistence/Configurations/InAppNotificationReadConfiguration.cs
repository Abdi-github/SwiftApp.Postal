using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Configurations;

public class InAppNotificationReadConfiguration : IEntityTypeConfiguration<InAppNotificationRead>
{
    public void Configure(EntityTypeBuilder<InAppNotificationRead> builder)
    {
        builder.ToTable("in_app_notification_reads");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");
        builder.Property(e => e.InAppNotificationId).HasColumnName("in_app_notification_id");
        builder.Property(e => e.EmployeeId).HasColumnName("employee_id");
        builder.Property(e => e.ReadAt).HasColumnName("read_at");

        builder.HasIndex(e => new { e.InAppNotificationId, e.EmployeeId }).IsUnique();

        builder.HasQueryFilter(r => r.InAppNotification.DeletedAt == null);
    }
}
