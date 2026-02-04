using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Address.Domain.Interfaces;

public interface ISwissAddressRepository
{
    Task<SwissAddress?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<SwissAddress>> GetByZipCodeAsync(string zipCode, CancellationToken ct = default);
    Task<List<SwissAddress>> GetByCantonAsync(string canton, CancellationToken ct = default);
    Task<PagedResult<SwissAddress>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(SwissAddress address, CancellationToken ct = default);
    Task UpdateAsync(SwissAddress address, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
