using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;

public interface IDeliveryRouteRepository
{
    Task<DeliveryRoute?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DeliveryRoute?> GetByCodeAsync(string routeCode, CancellationToken ct = default);
    Task<PagedResult<DeliveryRoute>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(DeliveryRoute route, CancellationToken ct = default);
    Task UpdateAsync(DeliveryRoute route, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
