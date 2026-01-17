using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Parcel.Infrastructure.Persistence.Repositories;

public class ParcelRepository(AppDbContext db) : IParcelRepository
{
    public async Task<Domain.Entities.Parcel?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<Domain.Entities.Parcel>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Domain.Entities.Parcel?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
        => await db.Set<Domain.Entities.Parcel>().FirstOrDefaultAsync(e => e.TrackingNumber == trackingNumber, ct);

    public async Task<PagedResult<Domain.Entities.Parcel>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<Domain.Entities.Parcel>().OrderByDescending(e => e.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<Domain.Entities.Parcel>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(Domain.Entities.Parcel parcel, CancellationToken ct = default)
    {
        await db.Set<Domain.Entities.Parcel>().AddAsync(parcel, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Domain.Entities.Parcel parcel, CancellationToken ct = default)
    {
        db.Set<Domain.Entities.Parcel>().Update(parcel);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<Domain.Entities.Parcel>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            // System.Diagnostics.Debug.WriteLine($"Soft deleting parcel entity {id}");
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
