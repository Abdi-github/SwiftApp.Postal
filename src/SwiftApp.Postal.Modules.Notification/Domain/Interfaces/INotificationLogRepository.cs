using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

public interface INotificationLogRepository
{
    Task<NotificationLog?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<NotificationLog>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task<List<NotificationLog>> GetFailedAsync(int maxRetries, int limit, CancellationToken ct = default);
    Task AddAsync(NotificationLog log, CancellationToken ct = default);
    Task UpdateAsync(NotificationLog log, CancellationToken ct = default);
}
