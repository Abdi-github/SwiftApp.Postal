using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;

public interface IParcelRepository
{
    Task<Entities.Parcel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Entities.Parcel?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<PagedResult<Entities.Parcel>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(Entities.Parcel parcel, CancellationToken ct = default);
    Task UpdateAsync(Entities.Parcel parcel, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
