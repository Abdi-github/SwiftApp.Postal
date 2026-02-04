namespace SwiftApp.Postal.Modules.Notification.Domain.Interfaces;

public interface IInAppNotificationRepository
{
    Task<Domain.Entities.InAppNotification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Domain.Entities.InAppNotification>> GetForEmployeeAsync(Guid employeeId, int limit, CancellationToken ct = default);
    Task<List<Domain.Entities.InAppNotification>> GetForRoleAsync(string role, int limit, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid employeeId, CancellationToken ct = default);
    Task AddAsync(Domain.Entities.InAppNotification notification, CancellationToken ct = default);
    Task MarkReadAsync(Guid notificationId, Guid employeeId, CancellationToken ct = default);
}
