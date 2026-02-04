using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Repositories;

public class NotificationLogRepository(AppDbContext db) : INotificationLogRepository
{
    public async Task<NotificationLog?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<NotificationLog>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<NotificationLog>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<NotificationLog>().OrderByDescending(e => e.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<NotificationLog>(items, page, size, total, totalPages);
    }

    public async Task<List<NotificationLog>> GetFailedAsync(int maxRetries, int limit, CancellationToken ct = default)
        => await db.Set<NotificationLog>()
            .Where(e => e.Status == NotificationStatus.Failed && e.RetryCount < maxRetries)
            .OrderBy(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task AddAsync(NotificationLog log, CancellationToken ct = default)
    {
        await db.Set<NotificationLog>().AddAsync(log, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(NotificationLog log, CancellationToken ct = default)
    {
        db.Set<NotificationLog>().Update(log);
        await db.SaveChangesAsync(ct);
    }
}
