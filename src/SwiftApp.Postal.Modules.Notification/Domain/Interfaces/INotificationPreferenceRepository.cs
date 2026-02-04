using SwiftApp.Postal.Modules.Notification.Domain.Entities;

namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<NotificationPreference?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task AddAsync(NotificationPreference preference, CancellationToken ct = default);
    Task UpdateAsync(NotificationPreference preference, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
