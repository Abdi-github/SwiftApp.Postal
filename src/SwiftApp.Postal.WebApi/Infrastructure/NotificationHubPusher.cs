using Microsoft.AspNetCore.SignalR;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.WebApi.Hubs;

namespace SwiftApp.Postal.WebApi.Infrastructure;

/// <summary>
/// Implements INotificationHubPusher by delegating to SignalR IHubContext.
/// </summary>
public class NotificationHubPusher(IHubContext<NotificationHub, INotificationClient> hubContext) : INotificationHubPusher
{
    public async Task PushToUserAsync(string userId, InAppNotificationResponse notification, CancellationToken ct = default)
        => await hubContext.Clients.Group($"user:{userId}").ReceiveNotification(notification);

    public async Task PushToRoleAsync(string role, InAppNotificationResponse notification, CancellationToken ct = default)
        => await hubContext.Clients.Group($"role:{role}").ReceiveNotification(notification);

    public async Task UpdateUnreadCountAsync(string userId, int count, CancellationToken ct = default)
        => await hubContext.Clients.Group($"user:{userId}").UpdateUnreadCount(count);
}
