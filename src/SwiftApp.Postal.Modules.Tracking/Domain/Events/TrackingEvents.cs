using SwiftApp.Postal.SharedKernel.Events;

namespace SwiftApp.Postal.Modules.Tracking.Domain.Events;

public record TrackingEventRecordedEvent(Guid TrackingEventId, string TrackingNumber, string EventType) : IDomainEvent;
