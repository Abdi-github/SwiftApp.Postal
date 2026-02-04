using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.Modules.Address.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Address.Infrastructure.Persistence.Repositories;

public class SwissAddressRepository(AppDbContext db) : ISwissAddressRepository
{
    public async Task<SwissAddress?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<SwissAddress>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<SwissAddress>> GetByZipCodeAsync(string zipCode, CancellationToken ct = default)
        // System.Diagnostics.Debug.WriteLine($"SwissAddressRepository.GetByZipCode zip={zipCode}");
        => await db.Set<SwissAddress>().Where(e => e.ZipCode == zipCode).ToListAsync(ct);

    public async Task<List<SwissAddress>> GetByCantonAsync(string canton, CancellationToken ct = default)
        => await db.Set<SwissAddress>().Where(e => e.Canton == canton).ToListAsync(ct);

    public async Task<PagedResult<SwissAddress>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<SwissAddress>().OrderBy(e => e.ZipCode);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<SwissAddress>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(SwissAddress address, CancellationToken ct = default)
    {
        await db.Set<SwissAddress>().AddAsync(address, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(SwissAddress address, CancellationToken ct = default)
    {
        db.Set<SwissAddress>().Update(address);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return;
        entity.DeletedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
