using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.Modules.Parcel.Domain.Events;

namespace SwiftApp.Postal.WebApi.Infrastructure.EventHandlers;

public class ParcelCreatedEventHandler(
    NotificationService notificationService,
    ILogger<ParcelCreatedEventHandler> logger)
    : INotificationHandler<ParcelCreatedEvent>
{
    public async Task Handle(ParcelCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending in-app notification for parcel {TrackingNumber}", notification.TrackingNumber);
        // System.Diagnostics.Debug.WriteLine($"ParcelCreatedEvent received: parcelId={notification.ParcelId}, tracking={notification.TrackingNumber}");
        try
        {
            await notificationService.SendInAppAsync(new InAppNotificationRequest(
                TargetEmployeeId: null,
                TargetRole: "EMPLOYEE",
                Title: "New parcel registered",
                Message: $"Parcel {notification.TrackingNumber} has been created and is awaiting acceptance.",
                Category: "Parcel",
                ReferenceUrl: $"/app/parcels/{notification.ParcelId}"),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send in-app notification for parcel {ParcelId}", notification.ParcelId);
        }
    }
}
