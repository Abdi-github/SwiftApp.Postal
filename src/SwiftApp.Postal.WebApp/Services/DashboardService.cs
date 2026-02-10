using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;

namespace SwiftApp.Postal.WebApp.Services;

public record DashboardStats(
    int TotalParcels,
    int ParcelsInTransit,
    int ActiveBranches,
    int ActiveEmployees,
    int ActiveCustomers,
    int ActiveRoutes,
    int PendingPickups);

public class DashboardService(
    IParcelModuleApi parcelApi,
    IBranchModuleApi branchApi,
    IAuthModuleApi authApi,
    IDeliveryModuleApi deliveryApi)
{
    public async Task<DashboardStats> GetStatsAsync(CancellationToken ct = default)
    {
        var total = await parcelApi.GetTotalParcelCountAsync(ct);
        var inTransit = await parcelApi.GetParcelsInTransitCountAsync(ct);
        var branches = await branchApi.GetActiveBranchCountAsync(ct);
        var employees = await authApi.GetActiveEmployeeCountAsync(ct);
        var customers = await authApi.GetActiveCustomerCountAsync(ct);
        var routes = await deliveryApi.GetActiveRouteCountAsync(ct);
        var pickups = await deliveryApi.GetPendingPickupCountAsync(ct);

        return new DashboardStats(total, inTransit, branches, employees, customers, routes, pickups);
    }
}
