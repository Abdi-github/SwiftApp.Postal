using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Entities;

public class PickupRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string PickupStreet { get; set; } = string.Empty;
    public string PickupZipCode { get; set; } = string.Empty;
    public string PickupCity { get; set; } = string.Empty;
    public DateOnly PreferredDate { get; set; }
    public TimeOnly? PreferredTimeFrom { get; set; }
    public TimeOnly? PreferredTimeTo { get; set; }
    public PickupStatus Status { get; set; } = PickupStatus.Pending;
}
