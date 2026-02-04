using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    public string TemplateCode { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? EventType { get; set; }
    public ICollection<NotificationTemplateTranslation> Translations { get; set; } = [];
}
