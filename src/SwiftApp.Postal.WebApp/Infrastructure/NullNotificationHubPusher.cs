using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

namespace SwiftApp.Postal.WebApp.Infrastructure;

/// <summary>
/// No-op implementation of INotificationHubPusher for the Blazor SSR host.
/// Real-time pushes are handled by WebApi's SignalR hub; WebApp reads from the DB.
/// </summary>
public class NullNotificationHubPusher : INotificationHubPusher
{
    public Task PushToUserAsync(string userId, InAppNotificationResponse notification, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task PushToRoleAsync(string role, InAppNotificationResponse notification, CancellationToken ct = default)
        => Task.CompletedTask;

    public Task UpdateUnreadCountAsync(string userId, int count, CancellationToken ct = default)
        => Task.CompletedTask;
}
