using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Auth.Domain.Events;
using SwiftApp.Postal.Modules.Notification.Application.Services;

namespace SwiftApp.Postal.WebApi.Infrastructure.EventHandlers;

public class CustomerCreatedEventHandler(
    NotificationService notificationService,
    ILogger<CustomerCreatedEventHandler> logger)
    : INotificationHandler<CustomerCreatedEvent>
{
    public async Task Handle(CustomerCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending welcome notification for new customer {CustomerNumber}", notification.CustomerNumber);
        try
        {
            await notificationService.SendTemplatedEmailAsync(
                notification.Email,
                notification.CustomerNumber,
                "customer-welcome",
                "de-CH",
                new { notification.CustomerNumber, notification.CustomerId },
                "CustomerCreated",
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send welcome notification for customer {CustomerId}", notification.CustomerId);
        }
    }
}
