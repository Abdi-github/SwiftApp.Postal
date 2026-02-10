using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.Modules.Parcel.Domain.Events;

namespace SwiftApp.Postal.WebApi.Infrastructure.EventHandlers;

public class ParcelStatusChangedEventHandler(
    NotificationService notificationService,
    ILogger<ParcelStatusChangedEventHandler> logger)
    : INotificationHandler<ParcelStatusChangedEvent>
{
    public async Task Handle(ParcelStatusChangedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Parcel {TrackingNumber} status: {Old} → {New}",
            notification.TrackingNumber, notification.OldStatus, notification.NewStatus);

        if (notification.NewStatus is not ("Delivered" or "DeliveryFailed" or "Returned"))
            return;

        try
        {
            await notificationService.SendInAppAsync(new InAppNotificationRequest(
                TargetEmployeeId: null,
                TargetRole: "BRANCH_MANAGER",
                Title: $"Parcel {notification.NewStatus.ToUpper()}",
                Message: $"Parcel {notification.TrackingNumber} status changed to {notification.NewStatus}.",
                Category: "Parcel",
                ReferenceUrl: $"/app/parcels/{notification.ParcelId}"),
                ct);
        }
        catch (Exception ex)
        {
            // System.Diagnostics.Debug.WriteLine($"Parcel status notification dispatch failed for parcelId={notification.ParcelId}");
            logger.LogWarning(ex, "Failed to send status notification for parcel {ParcelId}", notification.ParcelId);
        }
    }
}
