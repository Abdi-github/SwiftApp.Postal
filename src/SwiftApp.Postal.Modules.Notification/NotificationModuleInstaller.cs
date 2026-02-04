using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.Modules.Notification.Infrastructure.Email;
using SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Repositories;
using SwiftApp.Postal.SharedKernel.Interfaces;

namespace SwiftApp.Postal.Modules.Notification;

public class NotificationModuleInstaller : IModuleInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<IInAppNotificationRepository, InAppNotificationRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<NotificationService>();
    }
}
