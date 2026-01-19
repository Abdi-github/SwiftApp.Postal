using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Delivery.Application.DTOs;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.Modules.Delivery.Domain.Events;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Delivery.Application.Services;

public class DeliveryRouteService(
    IDeliveryRouteRepository routeRepository,
    IMediator mediator,
    ILogger<DeliveryRouteService> logger)
{
    public async Task<PagedResult<DeliveryRouteResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await routeRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<DeliveryRouteResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<DeliveryRouteResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var route = await routeRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(DeliveryRoute), id);
        return MapToResponse(route);
    }

    public async Task<DeliveryRouteResponse> CreateAsync(DeliveryRouteRequest request, CancellationToken ct = default)
    {
        // logger.LogDebug("Creating route for Branch={BranchId}, Employee={EmployeeId}, Slots={SlotCount}, Date={Date}", request.BranchId, request.AssignedEmployeeId, request.Slots.Count, request.Date);
        var route = new DeliveryRoute
        {
            Id = Guid.NewGuid(),
            RouteCode = GenerateRouteCode(),
            BranchId = request.BranchId,
            AssignedEmployeeId = request.AssignedEmployeeId,
            Status = RouteStatus.Planned,
            Date = request.Date,
            Slots = request.Slots.Select(s => new DeliverySlot
            {
                Id = Guid.NewGuid(),
                TrackingNumber = s.TrackingNumber,
                SequenceOrder = s.SequenceOrder,
                Status = SlotStatus.Pending
            }).ToList()
        };

        await routeRepository.AddAsync(route, ct);
        logger.LogInformation("Delivery route created: {RouteCode} with {SlotCount} slots", route.RouteCode, route.Slots.Count);

        return MapToResponse(route);
    }

    public async Task StartRouteAsync(Guid id, CancellationToken ct = default)
    {
        var route = await routeRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(DeliveryRoute), id);

        if (route.Status != RouteStatus.Planned)
            throw new BusinessRuleException("ROUTE_START", "Only planned routes can be started.");

        // System.Diagnostics.Debug.WriteLine($"Route start check passed for {route.RouteCode}. Current status={route.Status}");
        route.Status = RouteStatus.InProgress;
        await routeRepository.UpdateAsync(route, ct);
        logger.LogInformation("Delivery route started: {RouteCode}", route.RouteCode);
    }

    public async Task CompleteRouteAsync(Guid id, CancellationToken ct = default)
    {
        var route = await routeRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(DeliveryRoute), id);

        if (route.Status != RouteStatus.InProgress)
            throw new BusinessRuleException("ROUTE_COMPLETE", "Only in-progress routes can be completed.");

        route.Status = RouteStatus.Completed;
        await routeRepository.UpdateAsync(route, ct);
        await mediator.Publish(new DeliveryCompletedEvent(route.Id, route.RouteCode), ct);
        logger.LogInformation("Delivery route completed: {RouteCode}", route.RouteCode);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await routeRepository.SoftDeleteAsync(id, ct);
        logger.LogInformation("Delivery route soft-deleted: {Id}", id);
    }

    public async Task<DeliveryAttemptResponse> RecordAttemptAsync(Guid routeId, Guid slotId, DeliveryAttemptRequest request, CancellationToken ct = default)
    {
        var route = await routeRepository.GetByIdAsync(routeId, ct)
            ?? throw new EntityNotFoundException(nameof(DeliveryRoute), routeId);

        var slot = route.Slots.FirstOrDefault(s => s.Id == slotId)
            ?? throw new EntityNotFoundException(nameof(DeliverySlot), slotId);

        var result = Enum.Parse<AttemptResult>(request.Result);
        // logger.LogDebug("Attempt input: RouteId={RouteId}, SlotId={SlotId}, Result={Result}", routeId, slotId, request.Result);
        var attempt = new DeliveryAttempt
        {
            Id = Guid.NewGuid(),
            DeliverySlotId = slotId,
            Result = result,
            Notes = request.Notes,
            AttemptTimestamp = DateTimeOffset.UtcNow
        };

        slot.Attempts.Add(attempt);
        slot.Status = result == AttemptResult.Delivered ? SlotStatus.Delivered : SlotStatus.Failed;
    // System.Diagnostics.Debug.WriteLine($"Slot status after attempt: SlotId={slot.Id}, Attempts={slot.Attempts.Count}, Status={slot.Status}");
        await routeRepository.UpdateAsync(route, ct);

        logger.LogInformation("Delivery attempt recorded: Route {RouteCode}, Slot {SlotId}, Result {Result}",
            route.RouteCode, slotId, result);

        return new DeliveryAttemptResponse(attempt.Id, slotId, result.ToString(), attempt.Notes, attempt.AttemptTimestamp);
    }

    private static DeliveryRouteResponse MapToResponse(DeliveryRoute r) => new(
        r.Id, r.RouteCode, r.BranchId, r.AssignedEmployeeId,
        r.Status.ToString(), r.Date, r.Slots.Count, r.CreatedAt);

    private static string GenerateRouteCode() => $"RT-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}";
}
