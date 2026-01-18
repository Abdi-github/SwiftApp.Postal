namespace SwiftApp.Postal.Modules.Auth.Domain.Interfaces;

public interface IAuthModuleApi
{
    Task<string?> GetEmployeeNameAsync(Guid employeeId, CancellationToken ct = default);
    Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default);
    Task<string?> GetCustomerEmailAsync(Guid customerId, CancellationToken ct = default);
    Task<int> GetActiveEmployeeCountAsync(CancellationToken ct = default);
    Task<int> GetActiveCustomerCountAsync(CancellationToken ct = default);
    Task<Guid?> GetEmployeeIdByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct = default);
}
