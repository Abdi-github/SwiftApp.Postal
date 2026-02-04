using Microsoft.Extensions.Logging;
using Quartz;
using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Jobs;

/// <summary>
/// Retries email notifications that failed to send, up to 3 times.
/// Runs every 5 minutes.
/// </summary>
[DisallowConcurrentExecution]
public class RetryFailedNotificationsJob(
    INotificationLogRepository logRepository,
    IEmailService emailService,
    ILogger<RetryFailedNotificationsJob> logger) : IJob
{
    private const int MaxRetries = 3;
    private const int BatchSize = 20;

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        var failed = await logRepository.GetFailedAsync(MaxRetries, BatchSize, ct);
        // logger.LogDebug("RetryFailedNotificationsJob fetched {Count} failed logs with MaxRetries={MaxRetries}", failed.Count, MaxRetries);

        if (failed.Count == 0) return;

        logger.LogInformation("RetryFailedNotificationsJob: retrying {Count} failed notifications", failed.Count);
        // TODO: Introduce exponential backoff per message to avoid retry bursts on provider outage.

        foreach (var log in failed)
        {
            try
            {
                if (log.Type == NotificationType.Email && log.RecipientEmail is not null && log.Subject is not null && log.Body is not null)
                {
                    await emailService.SendAsync(log.RecipientEmail, log.RecipientEmail, log.Subject, log.Body, ct);
                    log.Status = NotificationStatus.Sent;
                    logger.LogInformation("Retry succeeded: {Subject} -> {Email}", log.Subject, log.RecipientEmail);
                }
                else
                {
                    log.Status = NotificationStatus.Sent;
                }
            }
            catch (Exception ex)
            {
                log.RetryCount++;
                // System.Diagnostics.Debug.WriteLine($"Retry failed for log={log.Id}, nextRetryCount={log.RetryCount}");
                if (log.RetryCount >= MaxRetries)
                    log.Status = NotificationStatus.PermanentlyFailed;
                logger.LogWarning(ex, "Retry {RetryCount} failed for {Subject} -> {Email}", log.RetryCount, log.Subject, log.RecipientEmail);
            }

            await logRepository.UpdateAsync(log, ct);
        }
    }
}
