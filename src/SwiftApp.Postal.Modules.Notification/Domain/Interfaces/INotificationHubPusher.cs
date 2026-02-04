using SwiftApp.Postal.Modules.Notification.Application.DTOs;

namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

/// <summary>
/// Abstraction for pushing real-time notifications to connected clients.
/// Implemented by the host (WebApi) using SignalR IHubContext.
/// </summary>
public interface INotificationHubPusher
{
    /// <summary>Sends a notification to a specific employee (by user ID).</summary>
    Task PushToUserAsync(string userId, InAppNotificationResponse notification, CancellationToken ct = default);

    /// <summary>Sends a notification to all members of a role group.</summary>
    Task PushToRoleAsync(string role, InAppNotificationResponse notification, CancellationToken ct = default);

    /// <summary>Updates the unread badge count for a specific employee.</summary>
    Task UpdateUnreadCountAsync(string userId, int count, CancellationToken ct = default);
}
