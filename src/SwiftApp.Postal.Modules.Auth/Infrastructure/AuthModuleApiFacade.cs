using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure;

public class AuthModuleApiFacade(AppDbContext db) : IAuthModuleApi
{
    public async Task<string?> GetEmployeeNameAsync(Guid employeeId, CancellationToken ct = default)
    {
        var emp = await db.Set<Employee>().FirstOrDefaultAsync(e => e.Id == employeeId, ct);
        return emp is not null ? $"{emp.FirstName} {emp.LastName}" : null;
    }

    public async Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default)
    {
        var cust = await db.Set<Customer>().FirstOrDefaultAsync(e => e.Id == customerId, ct);
        return cust is not null ? $"{cust.FirstName} {cust.LastName}" : null;
    }

    public async Task<string?> GetCustomerEmailAsync(Guid customerId, CancellationToken ct = default)
    {
        var cust = await db.Set<Customer>().FirstOrDefaultAsync(e => e.Id == customerId, ct);
        return cust?.Email;
    }

    public async Task<int> GetActiveEmployeeCountAsync(CancellationToken ct = default)
        => await db.Set<Employee>().CountAsync(ct);

    public async Task<int> GetActiveCustomerCountAsync(CancellationToken ct = default)
        => await db.Set<Customer>().CountAsync(ct);

    public async Task<Guid?> GetEmployeeIdByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct = default)
    {
        var emp = await db.Set<Employee>().FirstOrDefaultAsync(e => e.KeycloakUserId == keycloakUserId, ct);
        return emp?.Id;
    }
}
