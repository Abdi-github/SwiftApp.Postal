using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class NotificationTemplateTranslation : BaseTranslation
{
    public Guid NotificationTemplateId { get; set; }
    public NotificationTemplate NotificationTemplate { get; set; } = null!;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
