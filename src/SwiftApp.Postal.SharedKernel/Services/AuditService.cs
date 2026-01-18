using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Interfaces;
using SwiftApp.Postal.SharedKernel.Persistence;

namespace SwiftApp.Postal.SharedKernel.Services;

/// <summary>
/// Writes audit log entries for entity changes.
/// </summary>
public class AuditService(AppDbContext dbContext) : IAuditService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        string performedBy,
        string? details = null,
        CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedBy = performedBy,
            Details = details,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Set<AuditLog>().Add(log);

        await _dbContext.SaveChangesAsync(ct);
    }
}
