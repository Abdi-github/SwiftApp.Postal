using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Notification.Application.Services;

public class NotificationService(
    INotificationLogRepository logRepository,
    INotificationTemplateRepository templateRepository,
    IInAppNotificationRepository inAppRepository,
    INotificationPreferenceRepository preferenceRepository,
    IEmailService emailService,
    INotificationHubPusher hubPusher,
    ILogger<NotificationService> logger)
{
    public async Task<PagedResult<NotificationLogResponse>> GetLogsPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await logRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapLogToResponse).ToList();
        return new PagedResult<NotificationLogResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<List<NotificationTemplateResponse>> GetAllTemplatesAsync(CancellationToken ct = default)
    {
        var templates = await templateRepository.GetAllAsync(ct);
        return templates.Select(MapTemplateToResponse).ToList();
    }

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody, string? eventType, CancellationToken ct = default)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            RecipientEmail = toEmail,
            Type = NotificationType.Email,
            Status = NotificationStatus.Pending,
            Subject = subject,
            Body = htmlBody,
            EventType = eventType
        };
        await logRepository.AddAsync(log, ct);
        // logger.LogDebug("Email notification staged: LogId={LogId}, EventType={EventType}, Subject={Subject}", log.Id, eventType, subject);

        try
        {
            await emailService.SendAsync(toEmail, toName, subject, htmlBody, ct);

            log.Status = NotificationStatus.Sent;
            await logRepository.UpdateAsync(log, ct);
            logger.LogInformation("Email sent: {Subject} -> {RecipientEmail}", subject, toEmail);
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.RetryCount++;
            // System.Diagnostics.Debug.WriteLine($"Email send failed for log={log.Id}, retryCount={log.RetryCount}");
            await logRepository.UpdateAsync(log, ct);
            logger.LogError(ex, "Email failed: {Subject} -> {RecipientEmail}", subject, toEmail);
        }
    }

    public async Task SendTemplatedEmailAsync(string toEmail, string toName, string templateCode, string locale, object model, string? eventType, CancellationToken ct = default)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            RecipientEmail = toEmail,
            Type = NotificationType.Email,
            Status = NotificationStatus.Pending,
            Subject = templateCode,
            EventType = eventType
        };
        await logRepository.AddAsync(log, ct);

        try
        {
            await emailService.SendTemplatedAsync(toEmail, toName, templateCode, locale, model, ct);
            log.Status = NotificationStatus.Sent;
            await logRepository.UpdateAsync(log, ct);
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.RetryCount++;
            await logRepository.UpdateAsync(log, ct);
            logger.LogError(ex, "Templated email failed: {TemplateCode} -> {RecipientEmail}", templateCode, toEmail);
        }
    }

    public async Task SendInAppAsync(InAppNotificationRequest request, CancellationToken ct = default)
    {
        // logger.LogDebug("In-app send request: TargetEmployeeId={TargetEmployeeId}, TargetRole={TargetRole}, Category={Category}", request.TargetEmployeeId, request.TargetRole, request.Category);
        var notification = new InAppNotification
        {
            Id = Guid.NewGuid(),
            TargetEmployeeId = request.TargetEmployeeId,
            TargetRole = request.TargetRole,
            Title = request.Title,
            Message = request.Message,
            Category = Enum.TryParse<NotificationCategory>(request.Category, out var cat) ? cat : NotificationCategory.Info,
            ReferenceUrl = request.ReferenceUrl
        };
        await inAppRepository.AddAsync(notification, ct);
        logger.LogInformation("In-app notification sent: {Title}", request.Title);
        // TODO: Consider deduplication for repeated in-app notifications within a short time window.

        var response = new InAppNotificationResponse(
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Category.ToString(),
            notification.ReferenceUrl,
            IsRead: false,
            notification.CreatedAt);

        if (request.TargetEmployeeId.HasValue)
        {
            await hubPusher.PushToUserAsync(request.TargetEmployeeId.Value.ToString(), response, ct);
            var unread = await inAppRepository.GetUnreadCountAsync(request.TargetEmployeeId.Value, ct);
            await hubPusher.UpdateUnreadCountAsync(request.TargetEmployeeId.Value.ToString(), unread, ct);
        }
        else if (request.TargetRole is not null)
        {
            await hubPusher.PushToRoleAsync(request.TargetRole, response, ct);
        }
    }

    public async Task<List<InAppNotificationResponse>> GetInAppForEmployeeAsync(Guid employeeId, int limit = 20, CancellationToken ct = default)
    {
        var items = await inAppRepository.GetForEmployeeAsync(employeeId, limit, ct);
        return items.Select(n => MapInAppToResponse(n, employeeId)).ToList();
    }

    public async Task<int> GetUnreadInAppCountAsync(Guid employeeId, CancellationToken ct = default)
        => await inAppRepository.GetUnreadCountAsync(employeeId, ct);

    public async Task MarkInAppReadAsync(Guid notificationId, Guid employeeId, CancellationToken ct = default)
        => await inAppRepository.MarkReadAsync(notificationId, employeeId, ct);

    // --- Notification Preferences ---

    public async Task<NotificationPreferenceResponse?> GetPreferenceByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var pref = await preferenceRepository.GetByCustomerIdAsync(customerId, ct);
        return pref is null ? null : MapPreferenceToResponse(pref);
    }

    public async Task<NotificationPreferenceResponse> CreatePreferenceAsync(NotificationPreferenceRequest request, CancellationToken ct = default)
    {
        var existing = await preferenceRepository.GetByCustomerIdAsync(request.CustomerId, ct);
        if (existing is not null)
            throw new BusinessRuleException("PREFERENCE_EXISTS", "Notification preference already exists for this customer.");

        var pref = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            EmailEnabled = request.EmailEnabled,
            SmsEnabled = request.SmsEnabled,
            InAppEnabled = request.InAppEnabled,
            PreferredLocale = request.PreferredLocale
        };

        await preferenceRepository.AddAsync(pref, ct);
        logger.LogInformation("Notification preference created for customer {CustomerId}", pref.CustomerId);
        return MapPreferenceToResponse(pref);
    }

    public async Task<NotificationPreferenceResponse> UpdatePreferenceAsync(Guid id, NotificationPreferenceRequest request, CancellationToken ct = default)
    {
        var pref = await preferenceRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(NotificationPreference), id);

        pref.EmailEnabled = request.EmailEnabled;
        pref.SmsEnabled = request.SmsEnabled;
        pref.InAppEnabled = request.InAppEnabled;
        pref.PreferredLocale = request.PreferredLocale;

        await preferenceRepository.UpdateAsync(pref, ct);
        logger.LogInformation("Notification preference updated for customer {CustomerId}", pref.CustomerId);
        return MapPreferenceToResponse(pref);
    }

    public async Task DeletePreferenceAsync(Guid id, CancellationToken ct = default)
    {
        await preferenceRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Notification preference soft-deleted: {Id}", id);
    }

    private static NotificationPreferenceResponse MapPreferenceToResponse(NotificationPreference p) => new(
        p.Id, p.CustomerId, p.EmailEnabled, p.SmsEnabled, p.InAppEnabled, p.PreferredLocale);

    private static NotificationLogResponse MapLogToResponse(NotificationLog l) => new(
        l.Id, l.RecipientEmail, l.Type.ToString(), l.Status.ToString(),
        l.Subject, l.EventType, l.CreatedAt);

    private static NotificationTemplateResponse MapTemplateToResponse(NotificationTemplate t) => new(
        t.Id, t.TemplateCode, t.Type.ToString(), t.EventType,
        t.Translations.Select(tr => new NotificationTemplateTranslationResponse(
            tr.Locale, tr.Subject, tr.Body)).ToList());

    private static InAppNotificationResponse MapInAppToResponse(InAppNotification n, Guid employeeId) => new(
        n.Id, n.Title, n.Message, n.Category.ToString(), n.ReferenceUrl,
        n.ReadReceipts.Any(r => r.EmployeeId == employeeId),
        n.CreatedAt);
}
