using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Repositories;

public class InAppNotificationRepository(AppDbContext db) : IInAppNotificationRepository
{
    public async Task<InAppNotification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<InAppNotification>().Include(n => n.ReadReceipts).FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<List<InAppNotification>> GetForEmployeeAsync(Guid employeeId, int limit, CancellationToken ct = default)
        => await db.Set<InAppNotification>()
            .Include(n => n.ReadReceipts)
            .Where(n => n.TargetEmployeeId == employeeId || n.TargetEmployeeId == null)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<List<InAppNotification>> GetForRoleAsync(string role, int limit, CancellationToken ct = default)
        => await db.Set<InAppNotification>()
            .Include(n => n.ReadReceipts)
            .Where(n => n.TargetRole == role)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(Guid employeeId, CancellationToken ct = default)
        => await db.Set<InAppNotification>()
            .Where(n => n.TargetEmployeeId == employeeId || n.TargetEmployeeId == null)
            .Where(n => !n.ReadReceipts.Any(r => r.EmployeeId == employeeId))
            .CountAsync(ct);

    public async Task AddAsync(InAppNotification notification, CancellationToken ct = default)
    {
        await db.Set<InAppNotification>().AddAsync(notification, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task MarkReadAsync(Guid notificationId, Guid employeeId, CancellationToken ct = default)
    {
        var already = await db.Set<InAppNotificationRead>()
            .AnyAsync(r => r.InAppNotificationId == notificationId && r.EmployeeId == employeeId, ct);
        if (already) return;

        var read = new InAppNotificationRead
        {
            Id = Guid.NewGuid(),
            InAppNotificationId = notificationId,
            EmployeeId = employeeId,
            ReadAt = DateTimeOffset.UtcNow
        };
        await db.Set<InAppNotificationRead>().AddAsync(read, ct);
        await db.SaveChangesAsync(ct);
    }
}
