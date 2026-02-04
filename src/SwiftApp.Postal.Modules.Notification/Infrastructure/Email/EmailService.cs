using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Scriban;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Email;

public class EmailService(
    IConfiguration configuration,
    INotificationTemplateRepository templateRepository,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = BuildMessage(toEmail, toName, subject, htmlBody);
        await SendMessageAsync(message, ct);
    }

    public async Task SendTemplatedAsync(string toEmail, string toName, string templateCode, string locale, object model, CancellationToken ct = default)
    {
        var templates = await templateRepository.GetAllAsync(ct);
        var template = templates.FirstOrDefault(t => t.TemplateCode == templateCode);
        if (template is null)
        {
            logger.LogWarning("Email template not found: {TemplateCode}", templateCode);
            return;
        }

        var translation = template.Translations.FirstOrDefault(t => t.Locale == locale)
                       ?? template.Translations.FirstOrDefault(t => t.Locale == "de-CH")
                       ?? template.Translations.FirstOrDefault();

        if (translation is null)
        {
            logger.LogWarning("No translation found for template {TemplateCode} locale {Locale}", templateCode, locale);
            return;
        }

        var subjectTemplate = Template.Parse(translation.Subject);
        var bodyTemplate = Template.Parse(translation.Body);

        var subject = await subjectTemplate.RenderAsync(model);
        var htmlBody = await bodyTemplate.RenderAsync(model);

        await SendAsync(toEmail, toName, subject, htmlBody, ct);
    }

    private async Task SendMessageAsync(MimeMessage message, CancellationToken ct)
    {
        var host = configuration["Email:Host"] ?? "localhost";
        var port = int.Parse(configuration["Email:Port"] ?? "1025");
        var useSsl = bool.Parse(configuration["Email:UseSsl"] ?? "false");
        var username = configuration["Email:Username"];
        var password = configuration["Email:Password"];

        try
        {
            using var client = new SmtpClient();
            var secureOptions = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;
            await client.ConnectAsync(host, port, secureOptions, ct);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                await client.AuthenticateAsync(username, password, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            logger.LogInformation("Email sent: {Subject} -> {Recipient}", message.Subject, message.To);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Recipient}: {Subject}", message.To, message.Subject);
            throw;
        }
    }

    private MimeMessage BuildMessage(string toEmail, string toName, string subject, string htmlBody)
    {
        var fromEmail = configuration["Email:From"] ?? "noreply@swiftapp.postal.ch";
        var fromName = configuration["Email:FromName"] ?? "Swiss Postal";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }
}
