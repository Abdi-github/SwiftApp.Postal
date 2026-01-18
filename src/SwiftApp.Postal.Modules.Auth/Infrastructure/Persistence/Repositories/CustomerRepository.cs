using Microsoft.EntityFrameworkCore;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.Modules.Auth.Infrastructure.Persistence.Repositories;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Set<Customer>().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Set<Customer>().FirstOrDefaultAsync(e => e.Email == email, ct);

    public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber, CancellationToken ct = default)
        => await db.Set<Customer>().FirstOrDefaultAsync(e => e.CustomerNumber == customerNumber, ct);

    public async Task<Customer?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken ct = default)
        => await db.Set<Customer>().FirstOrDefaultAsync(e => e.KeycloakUserId == keycloakUserId, ct);

    public async Task<PagedResult<Customer>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var query = db.Set<Customer>().OrderBy(e => e.LastName);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling((double)total / size);
        return new PagedResult<Customer>(items, page, size, total, totalPages);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await db.Set<Customer>().AddAsync(customer, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        db.Set<Customer>().Update(customer);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Set<Customer>().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            // System.Diagnostics.Debug.WriteLine($"Customer soft delete request for id={id}");
            entity.DeletedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }
}
