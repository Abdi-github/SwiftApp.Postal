using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class NotificationLog : BaseEntity
{
    public string? RecipientEmail { get; set; }
    public string? RecipientPhone { get; set; }
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? ReferenceId { get; set; }
    public string? EventType { get; set; }
    public int RetryCount { get; set; }
}
