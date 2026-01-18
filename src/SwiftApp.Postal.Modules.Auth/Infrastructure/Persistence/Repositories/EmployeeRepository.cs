using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Repositories;

public class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<Employee>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Set<Employee>().FirstOrDefaultAsync(e => e.Email == email, ct);

    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken ct = default)
        => await db.Set<Employee>().FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber, ct);

    public async Task<Employee?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct = default)
        => await db.Set<Employee>().FirstOrDefaultAsync(e => e.KeycloakUserId == keycloakUserId, ct);

    public async Task<PagedResult<Employee>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<Employee>().OrderBy(e => e.LastName);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<Employee>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        await db.Set<Employee>().AddAsync(employee, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        db.Set<Employee>().Update(employee);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<Employee>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            // System.Diagnostics.Debug.WriteLine($"Employee soft delete request for id={id}");
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
