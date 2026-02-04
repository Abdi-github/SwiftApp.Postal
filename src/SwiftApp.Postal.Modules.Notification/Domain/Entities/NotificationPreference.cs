using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid CustomerId { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; }
    public bool InAppEnabled { get; set; } = true;
    public string PreferredLocale { get; set; } = "de";
}
