using SwiftApp.Postal.Modules.Notification.Application.DTOs;

namespace SwiftApp.Postal.WebApi.Hubs;

/// <summary>
/// Typed client contract for the notification hub.
/// </summary>
public interface INotificationClient
{
    /// <summary>Pushes a new in-app notification to the connected client.</summary>
    Task ReceiveNotification(InAppNotificationResponse notification);

    /// <summary>Updates the unread notification badge count.</summary>
    Task UpdateUnreadCount(int count);
}
