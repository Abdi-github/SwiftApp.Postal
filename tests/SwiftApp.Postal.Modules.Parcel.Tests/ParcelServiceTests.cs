using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Parcel.Application.DTOs;
using SwiftApp.Postal.Modules.Parcel.Application.Services;
using SwiftApp.Postal.Modules.Parcel.Domain.Entities;
using SwiftApp.Postal.Modules.Parcel.Domain.Enums;
using SwiftApp.Postal.Modules.Parcel.Domain.Events;
using SwiftApp.Postal.Modules.Parcel.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Parcel.Tests;

public class ParcelServiceTests
{
    private readonly Mock<IParcelRepository> _repoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<ParcelService>> _loggerMock = new();
    private readonly ParcelService _sut;

    public ParcelServiceTests()
    {
        _sut = new ParcelService(_repoMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateParcel_WhenRequestIsValid()
    {
        // Arrange
        var request = BuildParcelRequest("Standard");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Parcel>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Status.Should().Be("Created");
        result.Type.Should().Be("Standard");
        result.SenderName.Should().Be("Hans Muster");
        result.RecipientName.Should().Be("Anna Bosch");
        result.TrackingNumber.Should().NotBeNullOrEmpty();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Parcel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldPublishParcelCreatedEvent_WhenCreated()
    {
        // Arrange
        var request = BuildParcelRequest("Express");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Parcel>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.CreateAsync(request);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ParcelCreatedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("Standard", 1.0, 7.50)]
    [InlineData("Priority",  1.0, 10.20)]
    [InlineData("Express",  1.0, 12.80)]
    [InlineData("Registered", 1.0, 9.60)]
    [InlineData("Insured",    1.0, 16.00)]
    public async Task ShouldCalculateCorrectPrice_ForParcelType(string type, decimal weight, decimal expectedPrice)
    {
        // Arrange
        var request = BuildParcelRequest(type, weight);
        Domain.Entities.Parcel? savedParcel = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Parcel>(), It.IsAny<CancellationToken>()))
                 .Callback<Domain.Entities.Parcel, CancellationToken>((p, _) => savedParcel = p)
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Price.Should().Be(expectedPrice);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnParcel_WhenIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var parcel = BuildParcel(id, "CHE123456789012345");
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(parcel);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Id.Should().Be(id);
        result.TrackingNumber.Should().Be("CHE123456789012345");
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Parcel?)null);

        // Act
        var act = () => _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── GetByTrackingNumberAsync ──────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnParcel_WhenTrackingNumberExists()
    {
        // Arrange
        var trackingNumber = "CHE123456789012345";
        var parcel = BuildParcel(Guid.NewGuid(), trackingNumber);
        _repoMock.Setup(r => r.GetByTrackingNumberAsync(trackingNumber, It.IsAny<CancellationToken>())).ReturnsAsync(parcel);

        // Act
        var result = await _sut.GetByTrackingNumberAsync(trackingNumber);

        // Assert
        result.TrackingNumber.Should().Be(trackingNumber);
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenTrackingNumberDoesNotExist()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByTrackingNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Domain.Entities.Parcel?)null);

        // Act
        var act = () => _sut.GetByTrackingNumberAsync("NOTFOUND");

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── CancelAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCancelParcel_WhenStatusIsCreated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var parcel = BuildParcel(id, "CHE001", ParcelStatus.Created);
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(parcel);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Parcel>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _sut.CancelAsync(id);

        // Assert
        parcel.Status.Should().Be(ParcelStatus.Cancelled);
        _repoMock.Verify(r => r.UpdateAsync(parcel, It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ParcelStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowBusinessRuleException_WhenCancellingDeliveredParcel()
    {
        // Arrange
        var id = Guid.NewGuid();
        var parcel = BuildParcel(id, "CHE001", ParcelStatus.Delivered);
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(parcel);

        // Act
        var act = () => _sut.CancelAsync(id);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*cancel*");
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenCancellingNonExistentParcel()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Parcel?)null);

        // Act
        var act = () => _sut.CancelAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── GetPagedAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedResult_WithMappedResponses()
    {
        // Arrange
        var parcels = Enumerable.Range(1, 5).Select(i => BuildParcel(Guid.NewGuid(), $"CHE00{i}")).ToList();
        var pagedResult = new PagedResult<Domain.Entities.Parcel>(parcels, 1, 20, 5, 1);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetPagedAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalItems.Should().Be(5);
        result.TotalPages.Should().Be(1);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ParcelRequest BuildParcelRequest(string type, decimal weight = 1.0m) => new(
        type, weight, 20.0m, 15.0m, 10.0m,
        null, null,
        "Hans Muster", "Bahnhofstr. 1", "8001", "Zürich", null,
        "Anna Bosch", "Hauptgasse 5", "3011", "Bern", null);

    private static Domain.Entities.Parcel BuildParcel(Guid id, string trackingNumber, ParcelStatus status = ParcelStatus.Created) => new()
    {
        Id = id,
        TrackingNumber = trackingNumber,
        Status = status,
        Type = ParcelType.Standard,
        WeightKg = 1.0m,
        Price = 7.50m,
        SenderName = "Hans Muster",
        SenderStreet = "Bahnhofstr. 1",
        SenderZipCode = "8001",
        SenderCity = "Zürich",
        RecipientName = "Anna Bosch",
        RecipientStreet = "Hauptgasse 5",
        RecipientZipCode = "3011",
        RecipientCity = "Bern",
        CreatedAt = DateTimeOffset.UtcNow
    };
}
