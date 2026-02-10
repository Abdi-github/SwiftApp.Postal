using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SwiftApp.Postal.Modules.Notification.Application.Services;

namespace SwiftApp.Postal.WebApi.Hubs;

/// <summary>
/// Real-time notification hub. Authenticated employees connect and are added
/// to a personal group (their user ID) and to any role groups they belong to.
/// </summary>
[Authorize]
public class NotificationHub(
    NotificationService notificationService,
    ILogger<NotificationHub> logger) : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        // Add to role-based groups so targeted broadcasts work
        var user = Context.User;
        if (user?.IsInRole("ADMIN") is true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "role:ADMIN");
        if (user?.IsInRole("BRANCH_MANAGER") is true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "role:BRANCH_MANAGER");
        if (user?.IsInRole("EMPLOYEE") is true)
            await Groups.AddToGroupAsync(Context.ConnectionId, "role:EMPLOYEE");

        logger.LogInformation("NotificationHub: {UserId} connected ({ConnectionId})", userId, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("NotificationHub: {UserId} disconnected ({ConnectionId})", Context.UserIdentifier, Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>Called by client to get the current unread count on reconnect.</summary>
    public async Task RequestUnreadCount(CancellationToken ct = default)
    {
        var userId = Context.UserIdentifier;
        if (userId is null || !Guid.TryParse(userId, out var employeeId)) return;

        var count = await notificationService.GetUnreadInAppCountAsync(employeeId, ct);
        await Clients.Caller.UpdateUnreadCount(count);
    }
}
