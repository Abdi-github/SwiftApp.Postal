namespace SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;

public interface IDeliveryModuleApi
{
    Task<int> GetActiveRouteCountAsync(CancellationToken ct = default);
    Task<int> GetPendingPickupCountAsync(CancellationToken ct = default);
}
