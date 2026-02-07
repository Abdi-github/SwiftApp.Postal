using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Tracking.Infrastructure.Persistence.Repositories;

public class TrackingRecordRepository(AppDbContext db) : ITrackingRecordRepository
{
    public async Task<TrackingRecord?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<TrackingRecord>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<TrackingRecord?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
        => await db.Set<TrackingRecord>().FirstOrDefaultAsync(e => e.TrackingNumber == trackingNumber, ct);

    public async Task<PagedResult<TrackingRecord>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<TrackingRecord>().OrderByDescending(e => e.CreatedAt);
        var total = await query.CountAsync(ct);
        // System.Diagnostics.Debug.WriteLine($"TrackingRecordRepository paging page={page}, size={size}, total={total}");
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<TrackingRecord>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(TrackingRecord record, CancellationToken ct = default)
    {
        await db.Set<TrackingRecord>().AddAsync(record, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TrackingRecord record, CancellationToken ct = default)
    {
        db.Set<TrackingRecord>().Update(record);
        await db.SaveChangesAsync(ct);
    }
}
