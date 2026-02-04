using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class InAppNotification : BaseEntity
{
    public Guid? TargetEmployeeId { get; set; }
    public string? TargetRole { get; set; }
    public Guid? TargetBranchId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; } = NotificationCategory.Info;
    public string? ReferenceUrl { get; set; }
    public Guid? SenderEmployeeId { get; set; }
    public ICollection<InAppNotificationRead> ReadReceipts { get; set; } = [];
}
