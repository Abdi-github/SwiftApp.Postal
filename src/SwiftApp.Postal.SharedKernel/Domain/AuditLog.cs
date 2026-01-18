namespace SwiftApp.Postal.SharedKernel.Domain;

/// <summary>
/// Audit log entity for tracking all data changes across the platform.
/// </summary>
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = string.Empty;

    public string? Details { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
