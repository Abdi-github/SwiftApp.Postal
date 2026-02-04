namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default);
    Task SendTemplatedAsync(string toEmail, string toName, string templateCode, string locale, object model, CancellationToken ct = default);
}
