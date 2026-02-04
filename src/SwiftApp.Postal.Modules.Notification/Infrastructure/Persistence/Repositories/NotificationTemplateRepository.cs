using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Notification.Infrastructure.Persistence.Repositories;

public class NotificationTemplateRepository(AppDbContext db) : INotificationTemplateRepository
{
    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<NotificationTemplate>().Include(t => t.Translations).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<NotificationTemplate?> GetByCodeAsync(string templateCode, CancellationToken ct = default)
        => await db.Set<NotificationTemplate>().Include(t => t.Translations).FirstOrDefaultAsync(e => e.TemplateCode == templateCode, ct);

    public async Task<List<NotificationTemplate>> GetAllAsync(CancellationToken ct = default)
        => await db.Set<NotificationTemplate>().Include(t => t.Translations).ToListAsync(ct);

    public async Task AddAsync(NotificationTemplate template, CancellationToken ct = default)
    {
        await db.Set<NotificationTemplate>().AddAsync(template, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(NotificationTemplate template, CancellationToken ct = default)
    {
        db.Set<NotificationTemplate>().Update(template);
        await db.SaveChangesAsync(ct);
    }
}
