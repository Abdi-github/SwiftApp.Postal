using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Tracking.Domain.Entities;

public class TrackingRecord : BaseEntity
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public Guid? CurrentBranchId { get; set; }
    public DateTimeOffset? EstimatedDelivery { get; set; }
}
