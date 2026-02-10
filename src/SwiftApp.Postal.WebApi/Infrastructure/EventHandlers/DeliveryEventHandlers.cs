using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Delivery.Domain.Events;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;

namespace SwiftApp.Postal.WebApi.Infrastructure.EventHandlers;

public class DeliveryCompletedEventHandler(
    NotificationService notificationService,
    ILogger<DeliveryCompletedEventHandler> logger)
    : INotificationHandler<DeliveryCompletedEvent>
{
    public async Task Handle(DeliveryCompletedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Delivery completed for parcel {TrackingNumber}", notification.TrackingNumber);
        try
        {
            await notificationService.SendInAppAsync(new InAppNotificationRequest(
                TargetEmployeeId: null,
                TargetRole: "BRANCH_MANAGER",
                Title: "Parcel Delivered",
                Message: $"Parcel {notification.TrackingNumber} was successfully delivered.",
                Category: "Delivery",
                ReferenceUrl: null),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send delivery notification for slot {SlotId}", notification.DeliverySlotId);
        }
    }
}

public class DeliveryFailedEventHandler(
    NotificationService notificationService,
    ILogger<DeliveryFailedEventHandler> logger)
    : INotificationHandler<DeliveryFailedEvent>
{
    public async Task Handle(DeliveryFailedEvent notification, CancellationToken ct)
    {
        logger.LogWarning("Delivery failed for parcel {TrackingNumber}: {Reason}", notification.TrackingNumber, notification.Reason);
        try
        {
            await notificationService.SendInAppAsync(new InAppNotificationRequest(
                TargetEmployeeId: null,
                TargetRole: "BRANCH_MANAGER",
                Title: "Delivery Failed",
                Message: $"Parcel {notification.TrackingNumber} delivery failed. Reason: {notification.Reason}",
                Category: "Alert",
                ReferenceUrl: null),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send delivery-failed notification for slot {SlotId}", notification.DeliverySlotId);
        }
    }
}
