using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;

public interface ITrackingRecordRepository
{
    Task<TrackingRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TrackingRecord?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<PagedResult<TrackingRecord>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(TrackingRecord record, CancellationToken ct = default);
    Task UpdateAsync(TrackingRecord record, CancellationToken ct = default);
}
