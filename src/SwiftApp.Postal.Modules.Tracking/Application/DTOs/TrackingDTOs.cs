namespace SwiftApp.Postal.Modules.Tracking.Application.DTOs;

public record TrackingEventRequest(
    string TrackingNumber,
    string EventType,
    Guid? BranchId,
    string? Location,
    string? DescriptionKey);

public record TrackingTimelineResponse(
    string TrackingNumber,
    string CurrentStatus,
    DateTimeOffset? EstimatedDelivery,
    List<TrackingEventResponse> Events);

public record TrackingEventResponse(
    Guid Id,
    string EventType,
    string? Location,
    string? DescriptionKey,
    DateTimeOffset EventTimestamp);
