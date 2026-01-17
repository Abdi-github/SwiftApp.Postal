namespace SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;

public interface IParcelModuleApi
{
    Task<int> GetTotalParcelCountAsync(CancellationToken ct = default);
    Task<int> GetParcelsInTransitCountAsync(CancellationToken ct = default);
}
