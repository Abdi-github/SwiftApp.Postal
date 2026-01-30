namespace SwiftApp.Postal.Modules.Branch.Domain.Interfaces;

public interface IBranchModuleApi
{
    Task<string?> GetBranchNameAsync(Guid branchId, string locale, CancellationToken ct = default);
    Task<int> GetActiveBranchCountAsync(CancellationToken ct = default);
}
