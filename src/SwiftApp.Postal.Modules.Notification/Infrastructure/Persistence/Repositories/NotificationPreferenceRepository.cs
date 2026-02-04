using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Repositories;

public class NotificationPreferenceRepository(AppDbContext db) : INotificationPreferenceRepository
{
    public async Task<NotificationPreference?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<NotificationPreference>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<NotificationPreference?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await db.Set<NotificationPreference>().FirstOrDefaultAsync(e => e.CustomerId == customerId, ct);

    public async Task AddAsync(NotificationPreference preference, CancellationToken ct = default)
    {
        await db.Set<NotificationPreference>().AddAsync(preference, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(NotificationPreference preference, CancellationToken ct = default)
    {
        db.Set<NotificationPreference>().Update(preference);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<NotificationPreference>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
