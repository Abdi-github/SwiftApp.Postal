using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Tracking.Infrastructure.Persistence.Repositories;

public class TrackingEventRepository(AppDbContext db) : ITrackingEventRepository
{
    public async Task<List<TrackingEvent>> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
        // System.Diagnostics.Debug.WriteLine($"TrackingEventRepository.GetByTrackingNumber number={trackingNumber}");
        => await db.Set<TrackingEvent>()
            .Where(e => e.TrackingNumber == trackingNumber)
            .OrderByDescending(e => e.EventTimestamp)
            .ToListAsync(ct);

    public async Task AddAsync(TrackingEvent trackingEvent, CancellationToken ct = default)
    {
        await db.Set<TrackingEvent>().AddAsync(trackingEvent, ct);
        await db.SaveChangesAsync(ct);
    }
}
