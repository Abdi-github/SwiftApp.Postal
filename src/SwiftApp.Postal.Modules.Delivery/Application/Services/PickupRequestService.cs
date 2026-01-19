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

public class PickupRequestService(
    IPickupRequestRepository pickupRepository,
    IMediator mediator,
    ILogger<PickupRequestService> logger)
{
    public async Task<PagedResult<PickupRequestResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await pickupRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<PickupRequestResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<PickupRequestResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var request = await pickupRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(PickupRequest), id);
        return MapToResponse(request);
    }

    public async Task<PickupRequestResponse> CreateAsync(PickupRequestDto dto, CancellationToken ct = default)
    {
        // logger.LogDebug("Create pickup request: CustomerId={CustomerId}, PreferredDate={PreferredDate}, City={City}", dto.CustomerId, dto.PreferredDate, dto.PickupCity);
        var request = new PickupRequest
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            PickupStreet = dto.PickupStreet,
            PickupZipCode = dto.PickupZipCode,
            PickupCity = dto.PickupCity,
            PreferredDate = dto.PreferredDate,
            PreferredTimeFrom = dto.PreferredTimeFrom,
            PreferredTimeTo = dto.PreferredTimeTo,
            Status = PickupStatus.Pending
        };

        await pickupRepository.AddAsync(request, ct);
        logger.LogInformation("Pickup request created for customer {CustomerId}", request.CustomerId);

        await mediator.Publish(new PickupRequestCreatedEvent(request.Id, request.CustomerId), ct);

        return MapToResponse(request);
    }

    public async Task<PickupRequestResponse> TransitionStatusAsync(Guid id, string newStatus, CancellationToken ct = default)
    {
        var request = await pickupRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(PickupRequest), id);

        var targetStatus = Enum.Parse<PickupStatus>(newStatus);
        if (!CanTransitionPickup(request.Status, targetStatus))
            throw new BusinessRuleException("PICKUP_STATUS",
                $"Cannot transition from {request.Status} to {targetStatus}.");

        var oldStatus = request.Status.ToString();
        request.Status = targetStatus;

        await pickupRepository.UpdateAsync(request, ct);

        logger.LogInformation("Pickup request {Id} status changed: {OldStatus} → {NewStatus}", id, oldStatus, newStatus);

        return MapToResponse(request);
    }

    public async Task CancelAsync(Guid id, CancellationToken ct = default)
    {
        var request = await pickupRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(PickupRequest), id);

        if (request.Status is PickupStatus.PickedUp or PickupStatus.Cancelled)
            throw new BusinessRuleException("PICKUP_CANCEL", "Cannot cancel a pickup that is already picked up or cancelled.");

        request.Status = PickupStatus.Cancelled;
        await pickupRepository.UpdateAsync(request, ct);
        logger.LogInformation("Pickup request cancelled: {Id}", id);
    }

    private static bool CanTransitionPickup(PickupStatus from, PickupStatus to)
    {
        return (from, to) switch
        {
            (PickupStatus.Pending, PickupStatus.Requested) => true,
            (PickupStatus.Pending, PickupStatus.Confirmed) => true,
            (PickupStatus.Requested, PickupStatus.Confirmed) => true,
            (PickupStatus.Confirmed, PickupStatus.Assigned) => true,
            (PickupStatus.Assigned, PickupStatus.PickedUp) => true,
            (_, PickupStatus.Cancelled) when from is not (PickupStatus.PickedUp or PickupStatus.Cancelled) => true,
            _ => false,
        };
    }

    private static PickupRequestResponse MapToResponse(PickupRequest p) => new(
        p.Id, p.CustomerId, p.PickupStreet, p.PickupZipCode, p.PickupCity,
        p.PreferredDate, p.Status.ToString(), p.CreatedAt);
}
