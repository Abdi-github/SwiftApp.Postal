using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Delivery.Infrastructure;

public class DeliveryModuleApiFacade(AppDbContext db) : IDeliveryModuleApi
{
    public async Task<int> GetActiveRouteCountAsync(CancellationToken ct = default)
        => await db.Set<DeliveryRoute>().CountAsync(r => r.Status == RouteStatus.InProgress, ct);

    public async Task<int> GetPendingPickupCountAsync(CancellationToken ct = default)
        => await db.Set<PickupRequest>().CountAsync(p => p.Status == PickupStatus.Pending, ct);
}
