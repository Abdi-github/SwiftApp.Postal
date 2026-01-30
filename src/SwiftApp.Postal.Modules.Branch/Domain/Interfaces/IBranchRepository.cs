using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Branch.Domain.Interfaces;

public interface IBranchRepository
{
    Task<Entities.Branch?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Entities.Branch?> GetByCodeAsync(string branchCode, CancellationToken ct = default);
    Task<PagedResult<Entities.Branch>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(Entities.Branch branch, CancellationToken ct = default);
    Task UpdateAsync(Entities.Branch branch, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
