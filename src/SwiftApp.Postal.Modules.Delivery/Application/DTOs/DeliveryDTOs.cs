namespace SwiftApp.Postal.Modules.Delivery.Application.DTOs;

public record PickupStatusRequest(string Status);

public record DeliveryAttemptRequest(
    string Result,
    string? Notes);

public record DeliveryAttemptResponse(
    Guid Id,
    Guid DeliverySlotId,
    string Result,
    string? Notes,
    DateTimeOffset AttemptTimestamp);

public record DeliveryRouteRequest(
    Guid BranchId,
    Guid? AssignedEmployeeId,
    DateOnly Date,
    List<DeliverySlotRequest> Slots);

public record DeliverySlotRequest(
    string TrackingNumber,
    int SequenceOrder);

public record DeliveryRouteResponse(
    Guid Id,
    string RouteCode,
    Guid BranchId,
    Guid? AssignedEmployeeId,
    string Status,
    DateOnly Date,
    int TotalSlots,
    DateTimeOffset CreatedAt);

public record PickupRequestDto(
    Guid CustomerId,
    string PickupStreet,
    string PickupZipCode,
    string PickupCity,
    DateOnly PreferredDate,
    TimeOnly? PreferredTimeFrom,
    TimeOnly? PreferredTimeTo);

public record PickupRequestResponse(
    Guid Id,
    Guid CustomerId,
    string PickupStreet,
    string PickupZipCode,
    string PickupCity,
    DateOnly PreferredDate,
    string Status,
    DateTimeOffset CreatedAt);
