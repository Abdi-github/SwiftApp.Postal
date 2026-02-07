using SwiftApp.Postal.Modules.Tracking.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Tracking.Domain.Entities;

public class TrackingEvent : BaseEntity
{
    public string TrackingNumber { get; set; } = string.Empty;
    public TrackingEventType EventType { get; set; }
    public Guid? BranchId { get; set; }
    public string? Location { get; set; }
    public string? DescriptionKey { get; set; }
    public DateTimeOffset EventTimestamp { get; set; }
    public string? ScannedByEmployeeId { get; set; }
}
