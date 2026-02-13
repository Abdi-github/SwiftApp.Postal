using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Tracking.Application.DTOs;
using SwiftApp.Postal.Modules.Tracking.Application.Services;
using SwiftApp.Postal.Modules.Tracking.Domain.Entities;
using SwiftApp.Postal.Modules.Tracking.Domain.Enums;
using SwiftApp.Postal.Modules.Tracking.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Tracking.Tests;

public class TrackingServiceTests
{
    private readonly Mock<ITrackingRecordRepository> _recordRepoMock = new();
    private readonly Mock<ITrackingEventRepository> _eventRepoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<TrackingService>> _loggerMock = new();
    private readonly TrackingService _sut;

    public TrackingServiceTests()
    {
        _sut = new TrackingService(
            _recordRepoMock.Object,
            _eventRepoMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    // ── GetTimelineAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnTimeline_WhenTrackingNumberExists()
    {
        // Arrange
        var trackingNumber = "CHE123456789012345";
        var record = new TrackingRecord
        {
            Id = Guid.NewGuid(),
            TrackingNumber = trackingNumber,
            CurrentStatus = "InTransit",
            EstimatedDelivery = DateTimeOffset.UtcNow.AddDays(1)
        };
        var events = new List<TrackingEvent>
        {
            new() { Id = Guid.NewGuid(), TrackingNumber = trackingNumber,
                    EventType = TrackingEventType.Registered, Location = "Zürich", EventTimestamp = DateTimeOffset.UtcNow.AddHours(-2) },
            new() { Id = Guid.NewGuid(), TrackingNumber = trackingNumber,
                    EventType = TrackingEventType.ArrivedAtBranch, Location = "Bern", EventTimestamp = DateTimeOffset.UtcNow.AddHours(-1) }
        };
        _recordRepoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(record);
        _eventRepoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(events);

        // Act
        var result = await _sut.GetTimelineAsync(trackingNumber);

        // Assert
        result.TrackingNumber.Should().Be(trackingNumber);
        result.CurrentStatus.Should().Be("InTransit");
        result.Events.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenTrackingNumberNotFound()
    {
        // Arrange
        _recordRepoMock.Setup(r => r.GetByTrackingNumberAsync("UNKNOWN", It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TrackingRecord?)null);

        // Act
        var act = async () => await _sut.GetTimelineAsync("UNKNOWN");

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── RecordEventAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateRecord_WhenRecordDoesNotExistYet()
    {
        // Arrange
        var trackingNumber = "CHE999888777666555";
        var request = new TrackingEventRequest(trackingNumber, "Registered", null, "Zürich HB", null);

        _eventRepoMock.Setup(r => r.AddAsync(It.IsAny<TrackingEvent>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);
        _recordRepoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TrackingRecord?)null);
        _recordRepoMock.Setup(r => r.AddAsync(It.IsAny<TrackingRecord>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.RecordEventAsync(request, "emp-001");

        // Assert
        _recordRepoMock.Verify(r => r.AddAsync(It.IsAny<TrackingRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        _eventRepoMock.Verify(r => r.AddAsync(It.IsAny<TrackingEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldUpdateExistingRecord_WhenEventIsRecorded()
    {
        // Arrange
        var trackingNumber = "CHE111111111111111";
        var request = new TrackingEventRequest(trackingNumber, "ArrivedAtBranch", null, "Bern", null);
        var existingRecord = new TrackingRecord
        {
            Id = Guid.NewGuid(),
            TrackingNumber = trackingNumber,
            CurrentStatus = "Accepted"
        };

        _eventRepoMock.Setup(r => r.AddAsync(It.IsAny<TrackingEvent>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);
        _recordRepoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(existingRecord);
        _recordRepoMock.Setup(r => r.UpdateAsync(existingRecord, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.RecordEventAsync(request, "emp-scanner");

        // Assert
        existingRecord.CurrentStatus.Should().Be("ArrivedAtBranch");
        _recordRepoMock.Verify(r => r.UpdateAsync(existingRecord, It.IsAny<CancellationToken>()), Times.Once);
        _recordRepoMock.Verify(r => r.AddAsync(It.IsAny<TrackingRecord>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldStoreScannedBy_WhenEmployeeIsProvided()
    {
        // Arrange
        var trackingNumber = "CHE222222222222222";
        var request = new TrackingEventRequest(trackingNumber, "ArrivedAtSorting", null, "Zürich Sort Center", null);

        TrackingEvent? capturedEvent = null;
        _eventRepoMock.Setup(r => r.AddAsync(It.IsAny<TrackingEvent>(), It.IsAny<CancellationToken>()))
                      .Callback<TrackingEvent, CancellationToken>((e, _) => capturedEvent = e)
                      .Returns(Task.CompletedTask);
        _recordRepoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TrackingRecord?)null);
        _recordRepoMock.Setup(r => r.AddAsync(It.IsAny<TrackingRecord>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.RecordEventAsync(request, "emp-42");

        // Assert
        capturedEvent.Should().NotBeNull();
        capturedEvent!.ScannedByEmployeeId.Should().Be("emp-42");
    }
}
