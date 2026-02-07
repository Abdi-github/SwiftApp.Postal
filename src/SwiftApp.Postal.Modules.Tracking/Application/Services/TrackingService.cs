using MediatR;
using Microsoft.Extensions.Logging;
using SwiftApp.Postal.Modules.Tracking.Application.DTOs;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.Modules.Tracking.Domain.Enums;
using SwiftApp.Postal.Modules.Tracking.Domain.Events;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Exceptions;

namespace SwiftApp.Postal.Modules.Tracking.Application.Services;

public class TrackingService(
    ITrackingRecordRepository recordRepository,
    ITrackingEventRepository eventRepository,
    IMediator mediator,
    ILogger<TrackingService> logger)
{
    public async Task<TrackingTimelineResponse> GetTimelineAsync(string trackingNumber, CancellationToken ct = default)
    {
        var record = await recordRepository.GetByTrackingNumberAsync(trackingNumber, ct)
            ?? throw new EntityNotFoundException(nameof(TrackingRecord), Guid.Empty);

        var events = await eventRepository.GetByTrackingNumberAsync(trackingNumber, ct);

        return new TrackingTimelineResponse(
            record.TrackingNumber,
            record.CurrentStatus,
            record.EstimatedDelivery,
            events.Select(e => new TrackingEventResponse(
                e.Id, e.EventType.ToString(), e.Location, e.DescriptionKey, e.EventTimestamp)).ToList());
    }

    public async Task RecordEventAsync(TrackingEventRequest request, string? scannedBy, CancellationToken ct = default)
    {
        var eventType = Enum.Parse<TrackingEventType>(request.EventType);
        // logger.LogDebug("Tracking event request: TrackingNumber={TrackingNumber}, EventType={EventType}, Branch={BranchId}", request.TrackingNumber, request.EventType, request.BranchId);

        var trackingEvent = new TrackingEvent
        {
            Id = Guid.NewGuid(),
            TrackingNumber = request.TrackingNumber,
            EventType = eventType,
            BranchId = request.BranchId,
            Location = request.Location,
            DescriptionKey = request.DescriptionKey,
            EventTimestamp = DateTimeOffset.UtcNow,
            ScannedByEmployeeId = scannedBy
        };

        await eventRepository.AddAsync(trackingEvent, ct);

        // Update or create tracking record
        var record = await recordRepository.GetByTrackingNumberAsync(request.TrackingNumber, ct);
        if (record is null)
        {
            // System.Diagnostics.Debug.WriteLine($"Tracking record not found, creating new record for {request.TrackingNumber}");
            record = new TrackingRecord
            {
                Id = Guid.NewGuid(),
                TrackingNumber = request.TrackingNumber,
                CurrentStatus = eventType.ToString(),
                CurrentBranchId = request.BranchId
            };
            await recordRepository.AddAsync(record, ct);
        }
        else
        {
            record.CurrentStatus = eventType.ToString();
            record.CurrentBranchId = request.BranchId;
            await recordRepository.UpdateAsync(record, ct);
        }

        logger.LogInformation("Tracking event recorded: {TrackingNumber} -> {EventType}", request.TrackingNumber, eventType);

        await mediator.Publish(new TrackingEventRecordedEvent(trackingEvent.Id, request.TrackingNumber, eventType.ToString()), ct);
    }
}
