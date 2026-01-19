using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Entities;

public class DeliverySlot : BaseEntity
{
    public Guid DeliveryRouteId { get; set; }
    public DeliveryRoute DeliveryRoute { get; set; } = null!;
    public string TrackingNumber { get; set; } = string.Empty;
    public int SequenceOrder { get; set; }
    public SlotStatus Status { get; set; } = SlotStatus.Pending;
    public string? RecipientSignature { get; set; }
    public ICollection<DeliveryAttempt> Attempts { get; set; } = [];
}
