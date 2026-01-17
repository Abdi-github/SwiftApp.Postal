using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Parcel.Domain.Enums;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Parcel.Infrastructure;

public class ParcelModuleApiFacade(AppDbContext db) : IParcelModuleApi
{
    public async Task<int> GetTotalParcelCountAsync(CancellationToken ct = default)
        => await db.Set<Domain.Entities.Parcel>().CountAsync(ct);

    public async Task<int> GetParcelsInTransitCountAsync(CancellationToken ct = default)
        => await db.Set<Domain.Entities.Parcel>().CountAsync(p => p.Status == ParcelStatus.InTransit, ct);
}
