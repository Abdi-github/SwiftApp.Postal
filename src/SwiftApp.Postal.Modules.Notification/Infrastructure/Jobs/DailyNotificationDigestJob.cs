using Microsoft.Extensions.Logging;
using Quartz;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Jobs;

/// <summary>
/// Sends a daily digest summary of notification statistics to admins.
/// Runs daily at 07:00 Zurich time (06:00 UTC in winter, 05:00 UTC in summer).
/// </summary>
[DisallowConcurrentExecution]
public class DailyNotificationDigestJob(
    INotificationLogRepository logRepository,
    NotificationService notificationService,
    ILogger<DailyNotificationDigestJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        logger.LogInformation("DailyNotificationDigestJob: building digest");

        var paged = await logRepository.GetPagedAsync(1, 1000, ct);
        var today = DateTimeOffset.UtcNow.Date;

        var sent = paged.Items.Count(l => l.CreatedAt.Date == today && l.Status == Domain.Enums.NotificationStatus.Sent);
        var failed = paged.Items.Count(l => l.CreatedAt.Date == today && l.Status == Domain.Enums.NotificationStatus.Failed);
        var permanent = paged.Items.Count(l => l.CreatedAt.Date == today && l.Status == Domain.Enums.NotificationStatus.PermanentlyFailed);

        if (sent + failed + permanent == 0)
        {
            logger.LogInformation("DailyNotificationDigestJob: no activity today, skipping digest");
            return;
        }

        // TODO: Replace 1000-item fetch with date-bounded query for better scalability.

        var subject = $"[SwiftApp Postal] Daily Notification Digest — {today:dd.MM.yyyy}";
        var body = $"""
            <h2>SwiftApp Postal — Daily Notification Digest</h2>
            <p>Summary for {today:dd.MM.yyyy}:</p>
            <table border="1" cellpadding="6" style="border-collapse:collapse;">
              <tr><th>Status</th><th>Count</th></tr>
              <tr><td>✅ Sent</td><td>{sent}</td></tr>
              <tr><td>⚠️ Failed (will retry)</td><td>{failed}</td></tr>
              <tr><td>❌ Permanently Failed</td><td>{permanent}</td></tr>
            </table>
            """;

        await notificationService.SendEmailAsync(
            "admin@swiftapp.ch",
            "SwiftApp Admin",
            subject,
            body,
            "DailyDigest",
            ct);

        logger.LogInformation("DailyNotificationDigestJob: digest sent (sent={Sent}, failed={Failed}, permanent={Permanent})", sent, failed, permanent);
    }
}
