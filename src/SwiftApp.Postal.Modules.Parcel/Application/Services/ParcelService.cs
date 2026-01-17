using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Parcel.Application.DTOs;
using SwiftApp.Postal.Modules.Parcel.Domain;
using SwiftApp.Postal.Modules.Parcel.Domain.Enums;
using SwiftApp.Postal.Modules.Parcel.Domain.Events;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Parcel.Application.Services;

public class ParcelService(
    IParcelRepository parcelRepository,
    IMediator mediator,
    ILogger<ParcelService> logger)
{
    public async Task<PagedResult<ParcelResponse>> GetPagedAsync(int page, int size, CancellationToken ct = default)
    {
        var result = await parcelRepository.GetPagedAsync(page, size, ct);
        var items = result.Items.Select(MapToResponse).ToList();
        return new PagedResult<ParcelResponse>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<ParcelResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var parcel = await parcelRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Parcel), id);
        return MapToResponse(parcel);
    }

    public async Task<ParcelResponse> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
    {
        var parcel = await parcelRepository.GetByTrackingNumberAsync(trackingNumber, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Parcel), Guid.Empty);
        return MapToResponse(parcel);
    }

    public async Task<ParcelResponse> CreateAsync(ParcelRequest request, CancellationToken ct = default)
    {
        var parcelType = Enum.Parse<ParcelType>(request.Type);
        // logger.LogDebug("Create parcel request: Type={Type}, WeightKg={WeightKg}, OriginBranch={OriginBranchId}", request.Type, request.WeightKg, request.OriginBranchId);
        var parcel = new Domain.Entities.Parcel
        {
            Id = Guid.NewGuid(),
            TrackingNumber = GenerateTrackingNumber(),
            // TODO: Validate tracking number uniqueness to prevent duplicates
            Status = ParcelStatus.Created,
            Type = parcelType,
            WeightKg = request.WeightKg,
            LengthCm = request.LengthCm,
            WidthCm = request.WidthCm,
            HeightCm = request.HeightCm,
            Price = CalculatePrice(parcelType, request.WeightKg),
            SenderCustomerId = request.SenderCustomerId,
            OriginBranchId = request.OriginBranchId,
            SenderName = request.SenderName,
            SenderStreet = request.SenderStreet,
            SenderZipCode = request.SenderZipCode,
            SenderCity = request.SenderCity,
            SenderPhone = request.SenderPhone,
            RecipientName = request.RecipientName,
            RecipientStreet = request.RecipientStreet,
            RecipientZipCode = request.RecipientZipCode,
            RecipientCity = request.RecipientCity,
            RecipientPhone = request.RecipientPhone
        };

        await parcelRepository.AddAsync(parcel, ct);
        logger.LogInformation("Parcel created: {TrackingNumber}, Type={Type}, Price={Price}", parcel.TrackingNumber, parcel.Type, parcel.Price);
        // System.Diagnostics.Debug.WriteLine($"Parcel persisted with Id={parcel.Id}, TrackingNumber={parcel.TrackingNumber}");

        await mediator.Publish(new ParcelCreatedEvent(parcel.Id, parcel.TrackingNumber), ct);
        // logger.LogDebug("Published ParcelCreatedEvent for ParcelId={ParcelId}", parcel.Id);

        return MapToResponse(parcel);
    }

    public async Task CancelAsync(Guid id, CancellationToken ct = default)
    {
        var parcel = await parcelRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Parcel), id);

        if (parcel.Status is not (ParcelStatus.Created or ParcelStatus.LabelGenerated))
            throw new BusinessRuleException("PARCEL_CANCEL", "Only parcels in Created or LabelGenerated status can be cancelled.");

        var oldStatus = parcel.Status.ToString();
        parcel.Status = ParcelStatus.Cancelled;

        await parcelRepository.UpdateAsync(parcel, ct);

        logger.LogInformation("Parcel cancelled: {TrackingNumber}", parcel.TrackingNumber);
        await mediator.Publish(new ParcelStatusChangedEvent(parcel.Id, parcel.TrackingNumber, oldStatus, "Cancelled"), ct);
    }

    public async Task<ParcelResponse> UpdateAsync(Guid id, ParcelRequest request, CancellationToken ct = default)
    {
        var parcel = await parcelRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Parcel), id);

        if (parcel.Status is not (ParcelStatus.Created or ParcelStatus.LabelGenerated))
            throw new BusinessRuleException("PARCEL_UPDATE", "Only parcels in Created or LabelGenerated status can be updated.");

        var parcelType = Enum.Parse<ParcelType>(request.Type);
        parcel.Type = parcelType;
        parcel.WeightKg = request.WeightKg;
        parcel.LengthCm = request.LengthCm;
        parcel.WidthCm = request.WidthCm;
        parcel.HeightCm = request.HeightCm;
        parcel.Price = CalculatePrice(parcelType, request.WeightKg);
        // System.Diagnostics.Debug.WriteLine($"Recalculated price for {parcel.TrackingNumber}: {parcel.Price}");
        parcel.SenderCustomerId = request.SenderCustomerId;
        parcel.OriginBranchId = request.OriginBranchId;
        parcel.SenderName = request.SenderName;
        parcel.SenderStreet = request.SenderStreet;
        parcel.SenderZipCode = request.SenderZipCode;
        parcel.SenderCity = request.SenderCity;
        parcel.SenderPhone = request.SenderPhone;
        parcel.RecipientName = request.RecipientName;
        parcel.RecipientStreet = request.RecipientStreet;
        parcel.RecipientZipCode = request.RecipientZipCode;
        parcel.RecipientCity = request.RecipientCity;
        parcel.RecipientPhone = request.RecipientPhone;

        await parcelRepository.UpdateAsync(parcel, ct);
        logger.LogInformation("Parcel updated: {TrackingNumber}", parcel.TrackingNumber);

        return MapToResponse(parcel);
    }

    public async Task<ParcelResponse> TransitionStatusAsync(Guid id, string newStatus, CancellationToken ct = default)
    {
        var parcel = await parcelRepository.GetByIdAsync(id, ct)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Parcel), id);

        var targetStatus = Enum.Parse<ParcelStatus>(newStatus);
        if (!ParcelStatusMachine.CanTransition(parcel.Status, targetStatus))
            throw new BusinessRuleException("PARCEL_STATUS",
                $"Cannot transition from {parcel.Status} to {targetStatus}. Allowed: {string.Join(", ", ParcelStatusMachine.GetAllowedTransitions(parcel.Status))}");

        var oldStatus = parcel.Status.ToString();
        parcel.Status = targetStatus;
        await parcelRepository.UpdateAsync(parcel, ct);

        logger.LogInformation("Parcel status changed: {TrackingNumber} {OldStatus} → {NewStatus}", parcel.TrackingNumber, oldStatus, newStatus);
        await mediator.Publish(new ParcelStatusChangedEvent(parcel.Id, parcel.TrackingNumber, oldStatus, newStatus), ct);

        return MapToResponse(parcel);
    }

    private static decimal CalculatePrice(ParcelType type, decimal? weightKg) => type switch
    {
        ParcelType.Standard => 7.00m + (weightKg ?? 0m) * 0.50m,
        ParcelType.Priority => 9.50m + (weightKg ?? 0m) * 0.70m,
        ParcelType.Express => 12.00m + (weightKg ?? 0m) * 0.80m,
        ParcelType.Registered => 9.00m + (weightKg ?? 0m) * 0.60m,
        ParcelType.Insured => 15.00m + (weightKg ?? 0m) * 1.00m,
        _ => 7.00m
    };

    private static string GenerateTrackingNumber() => $"99.{Random.Shared.Next(10, 100)}.{Random.Shared.Next(100, 1000)}.{Random.Shared.Next(100, 1000)}.{Random.Shared.Next(100, 1000)}.{Random.Shared.Next(10, 100)}";

    private static ParcelResponse MapToResponse(Domain.Entities.Parcel p) => new(
        p.Id, p.TrackingNumber, p.Status.ToString(), p.Type.ToString(),
        p.WeightKg, p.Price,
        p.SenderName, p.SenderZipCode, p.SenderCity,
        p.RecipientName, p.RecipientZipCode, p.RecipientCity,
        p.CreatedAt);
}
