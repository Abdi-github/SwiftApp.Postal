using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Branch.Infrastructure.Persistence.Repositories;

public class BranchRepository(AppDbContext db) : IBranchRepository
{
    public async Task<Domain.Entities.Branch?> GetByIdAsync(Guid id, CancellationToken ct = default)
        // System.Diagnostics.Debug.WriteLine($"BranchRepository.GetById with translations include for id={id}");
        => await db.Set<Domain.Entities.Branch>().Include(b => b.Translations).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Domain.Entities.Branch?> GetByCodeAsync(string branchCode, CancellationToken ct = default)
        => await db.Set<Domain.Entities.Branch>().Include(b => b.Translations).FirstOrDefaultAsync(e => e.BranchCode == branchCode, ct);

    public async Task<PagedResult<Domain.Entities.Branch>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<Domain.Entities.Branch>().Include(b => b.Translations).OrderBy(e => e.BranchCode);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<Domain.Entities.Branch>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(Domain.Entities.Branch branch, CancellationToken ct = default)
    {
        await db.Set<Domain.Entities.Branch>().AddAsync(branch, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Domain.Entities.Branch branch, CancellationToken ct = default)
    {
        db.Set<Domain.Entities.Branch>().Update(branch);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<Domain.Entities.Branch>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
