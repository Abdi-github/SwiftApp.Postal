namespace SwiftApp.Postal.Modules.Notification.Application.DTOs;

public record NotificationPreferenceRequest(
    Guid CustomerId,
    bool EmailEnabled,
    bool SmsEnabled,
    bool InAppEnabled,
    string PreferredLocale);

public record NotificationPreferenceResponse(
    Guid Id,
    Guid CustomerId,
    bool EmailEnabled,
    bool SmsEnabled,
    bool InAppEnabled,
    string PreferredLocale);

public record NotificationLogResponse(
    Guid Id,
    string? RecipientEmail,
    string Type,
    string Status,
    string? Subject,
    string? EventType,
    DateTimeOffset CreatedAt);

public record NotificationTemplateResponse(
    Guid Id,
    string TemplateCode,
    string Type,
    string? EventType,
    List<NotificationTemplateTranslationResponse> Translations);

public record NotificationTemplateTranslationResponse(
    string Locale,
    string Subject,
    string Body);

public record SendEmailRequest(
    string ToEmail,
    string ToName,
    string Subject,
    string HtmlBody);

public record InAppNotificationRequest(
    Guid? TargetEmployeeId,
    string? TargetRole,
    string Title,
    string Message,
    string Category = "Info",
    string? ReferenceUrl = null);

public record InAppNotificationResponse(
    Guid Id,
    string Title,
    string Message,
    string Category,
    string? ReferenceUrl,
    bool IsRead,
    DateTimeOffset CreatedAt);
