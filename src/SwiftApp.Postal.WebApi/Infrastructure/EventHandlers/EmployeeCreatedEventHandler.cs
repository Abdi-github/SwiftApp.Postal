using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Auth.Domain.Events;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;

namespace SwiftApp.Postal.WebApi.Infrastructure.EventHandlers;

public class EmployeeCreatedEventHandler(
    NotificationService notificationService,
    ILogger<EmployeeCreatedEventHandler> logger)
    : INotificationHandler<EmployeeCreatedEvent>
{
    public async Task Handle(EmployeeCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Sending welcome notification for new employee {EmployeeNumber}", notification.EmployeeNumber);
        // logger.LogDebug("quick check new employee event: employeeId={EmployeeId}, employeeNumber={EmployeeNumber}", notification.EmployeeId, notification.EmployeeNumber);
        try
        {
            await notificationService.SendTemplatedEmailAsync(
                notification.Email,
                notification.EmployeeNumber,
                "employee-welcome",
                "de-CH",
                new { notification.EmployeeNumber, notification.EmployeeId },
                "EmployeeCreated",
                ct);
            // TODO: Select locale dynamically from employee profile instead of fixed de-CH.
        }
        catch (Exception ex)
        {
            // System.Diagnostics.Debug.WriteLine($"Welcome message dispatch failed for employeeId={notification.EmployeeId}");
            logger.LogWarning(ex, "Failed to send welcome notification for employee {EmployeeId}", notification.EmployeeId);
        }
    }
}
