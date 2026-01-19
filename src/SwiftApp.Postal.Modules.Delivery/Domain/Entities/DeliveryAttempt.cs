using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Entities;

public class DeliveryAttempt : BaseEntity
{
    public Guid DeliverySlotId { get; set; }
    public DeliverySlot DeliverySlot { get; set; } = null!;
    public AttemptResult Result { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset AttemptTimestamp { get; set; }
}
