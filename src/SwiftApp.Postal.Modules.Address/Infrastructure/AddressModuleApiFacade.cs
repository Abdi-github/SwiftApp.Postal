using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.Modules.Address.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Address.Infrastructure;

public class AddressModuleApiFacade(AppDbContext db) : IAddressModuleApi
{
    public async Task<bool> IsValidZipCodeAsync(string zipCode, CancellationToken ct = default)
        => await db.Set<SwissAddress>().AnyAsync(a => a.ZipCode == zipCode, ct);

    public async Task<string?> GetCantonByZipCodeAsync(string zipCode, CancellationToken ct = default)
    {
        var address = await db.Set<SwissAddress>().FirstOrDefaultAsync(a => a.ZipCode == zipCode, ct);
        return address?.Canton;
    }
}
