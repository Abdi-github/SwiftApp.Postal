namespace SwiftApp.Postal.SharedKernel.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        string performedBy,
        string? details = null,
        CancellationToken ct = default);
}
