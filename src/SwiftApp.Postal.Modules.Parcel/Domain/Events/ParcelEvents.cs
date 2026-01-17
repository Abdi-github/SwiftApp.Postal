using SwiftApp.Postal.SharedKernel.Events;

namespace SwiftApp.Postal.Modules.Parcel.Domain.Events;

public record ParcelCreatedEvent(Guid ParcelId, string TrackingNumber) : IDomainEvent;
public record ParcelStatusChangedEvent(Guid ParcelId, string TrackingNumber, string OldStatus, string NewStatus) : IDomainEvent;
