using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Repositories;

public class PickupRequestRepository(AppDbContext db) : IPickupRequestRepository
{
    public async Task<PickupRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<PickupRequest>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<PickupRequest>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<PickupRequest>().OrderByDescending(e => e.PreferredDate);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<PickupRequest>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(PickupRequest request, CancellationToken ct = default)
    {
        await db.Set<PickupRequest>().AddAsync(request, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PickupRequest request, CancellationToken ct = default)
    {
        db.Set<PickupRequest>().Update(request);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<PickupRequest>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            // System.Diagnostics.Debug.WriteLine($"PickupRequest soft delete request for id={id}");
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
