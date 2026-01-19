using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Entities;

public class DeliveryRoute : BaseEntity
{
    public string RouteCode { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public Guid? AssignedEmployeeId { get; set; }
    public RouteStatus Status { get; set; } = RouteStatus.Planned;
    public DateOnly Date { get; set; }
    public ICollection<DeliverySlot> Slots { get; set; } = [];
}
