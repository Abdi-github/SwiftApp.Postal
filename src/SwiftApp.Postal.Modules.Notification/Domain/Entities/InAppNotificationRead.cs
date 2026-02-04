namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class InAppNotificationRead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InAppNotificationId { get; set; }
    public InAppNotification InAppNotification { get; set; } = null!;
    public Guid EmployeeId { get; set; }
    public DateTimeOffset ReadAt { get; set; }
}
