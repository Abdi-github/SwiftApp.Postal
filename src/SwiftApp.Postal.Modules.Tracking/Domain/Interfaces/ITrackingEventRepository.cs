using SwiftApp.Postal.Modules.Tracking.Domain.Entities;

namespace SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;

public interface ITrackingEventRepository
{
    Task<List<TrackingEvent>> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task AddAsync(TrackingEvent trackingEvent, CancellationToken ct = default);
}
