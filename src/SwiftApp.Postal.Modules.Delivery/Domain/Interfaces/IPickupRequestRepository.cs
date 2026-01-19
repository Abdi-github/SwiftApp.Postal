using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;

public interface IPickupRequestRepository
{
    Task<PickupRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<PickupRequest>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(PickupRequest request, CancellationToken ct = default);
    Task UpdateAsync(PickupRequest request, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
