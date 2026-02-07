using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Tracking.Infrastructure;

public class TrackingModuleApiFacade(AppDbContext db) : ITrackingModuleApi
{
    public async Task<string?> GetCurrentStatusAsync(string trackingNumber, CancellationToken ct = default)
    {
        var record = await db.Set<TrackingRecord>().FirstOrDefaultAsync(r => r.TrackingNumber == trackingNumber, ct);
        return record?.CurrentStatus;
    }
}
