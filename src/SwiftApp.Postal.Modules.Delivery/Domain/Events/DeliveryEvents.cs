using SwiftApp.Postal.SharedKernel.Events;

namespace SwiftApp.Postal.Modules.Delivery.Domain.Events;

public record DeliveryCompletedEvent(Guid DeliverySlotId, string TrackingNumber) : IDomainEvent;
public record DeliveryFailedEvent(Guid DeliverySlotId, string TrackingNumber, string Reason) : IDomainEvent;
public record PickupRequestCreatedEvent(Guid PickupRequestId, Guid CustomerId) : IDomainEvent;
