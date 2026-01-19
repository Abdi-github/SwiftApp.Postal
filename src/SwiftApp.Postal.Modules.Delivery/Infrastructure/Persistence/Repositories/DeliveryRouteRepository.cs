using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure.Persistence.Repositories;

public class DeliveryRouteRepository(AppDbContext db) : IDeliveryRouteRepository
{
    public async Task<DeliveryRoute?> GetByIdAsync(Guid id, CancellationToken ct = default)
        // System.Diagnostics.Debug.WriteLine($"DeliveryRouteRepository.GetById with Slots+Attempts include graph for routeId={id}");
        => await db.Set<DeliveryRoute>().Include(r => r.Slots).ThenInclude(s => s.Attempts).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<DeliveryRoute?> GetByCodeAsync(string routeCode, CancellationToken ct = default)
        => await db.Set<DeliveryRoute>().Include(r => r.Slots).FirstOrDefaultAsync(e => e.RouteCode == routeCode, ct);

    public async Task<PagedResult<DeliveryRoute>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<DeliveryRoute>().OrderByDescending(e => e.Date);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<DeliveryRoute>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(DeliveryRoute route, CancellationToken ct = default)
    {
        await db.Set<DeliveryRoute>().AddAsync(route, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DeliveryRoute route, CancellationToken ct = default)
    {
        db.Set<DeliveryRoute>().Update(route);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<DeliveryRoute>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
