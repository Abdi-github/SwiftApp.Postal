using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;
using SwiftApp.Postal.SharedKernel.Services;

namespace SwiftApp.Postal.Modules.Branch.Infrastructure;

public class BranchModuleApiFacade(AppDbContext db) : IBranchModuleApi
{
    public async Task<string?> GetBranchNameAsync(Guid branchId, string locale, CancellationToken ct = default)
    {
        var branch = await db.Set<Domain.Entities.Branch>()
            .Include(b => b.Translations)
            .FirstOrDefaultAsync(b => b.Id == branchId, ct);

        if (branch is null) return null;

        return TranslationResolver.Resolve(branch.Translations, locale, t => t.Name);
    }

    public async Task<int> GetActiveBranchCountAsync(CancellationToken ct = default)
        => await db.Set<Domain.Entities.Branch>().CountAsync(ct);
}
