using SwiftApp.Postal.Modules.Notification.Domain.Entities;

namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<NotificationTemplate?> GetByCodeAsync(string templateCode, CancellationToken ct = default);
    Task<List<NotificationTemplate>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(NotificationTemplate template, CancellationToken ct = default);
    Task UpdateAsync(NotificationTemplate template, CancellationToken ct = default);
}
