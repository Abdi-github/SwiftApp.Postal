using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Auth.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken ct = default);
    Task<Employee?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct = default);
    Task<PagedResult<Employee>> GetPagedAsync(int page, int size, CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
    Task UpdateAsync(Employee employee, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
