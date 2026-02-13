using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Delivery.Application.DTOs;
using SwiftApp.Postal.Modules.Delivery.Application.Services;
using SwiftApp.Postal.Modules.Delivery.Domain.Entities;
using SwiftApp.Postal.Modules.Delivery.Domain.Enums;
using SwiftApp.Postal.Modules.Delivery.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Delivery.Tests;

public class DeliveryRouteServiceTests
{
    private readonly Mock<IDeliveryRouteRepository> _repoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<DeliveryRouteService>> _loggerMock = new();
    private readonly DeliveryRouteService _sut;

    public DeliveryRouteServiceTests()
    {
        _sut = new DeliveryRouteService(_repoMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateRoute_WhenRequestIsValid()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var request = new DeliveryRouteRequest(
            branchId,
            null,
            DateOnly.FromDateTime(DateTime.Today),
            [new DeliverySlotRequest("CHE123456789012345", 1)]);

        _repoMock.Setup(r => r.AddAsync(It.IsAny<DeliveryRoute>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.RouteCode.Should().StartWith("RT-");
        result.BranchId.Should().Be(branchId);
        result.Status.Should().Be("Planned");
        result.TotalSlots.Should().Be(1);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<DeliveryRoute>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldCreateMultipleSlots_WhenRequestContainsMany()
    {
        // Arrange
        var request = new DeliveryRouteRequest(
            Guid.NewGuid(), null, DateOnly.FromDateTime(DateTime.Today),
            [
                new DeliverySlotRequest("CHE111111111111111", 1),
                new DeliverySlotRequest("CHE222222222222222", 2),
                new DeliverySlotRequest("CHE333333333333333", 3)
            ]);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<DeliveryRoute>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.TotalSlots.Should().Be(3);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnRoute_WhenIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route = new DeliveryRoute
        {
            Id = id,
            RouteCode = "RT-20260411-001",
            BranchId = Guid.NewGuid(),
            Status = RouteStatus.Planned,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Slots = []
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(route);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Id.Should().Be(id);
        result.RouteCode.Should().Be("RT-20260411-001");
        result.Status.Should().Be("Planned");
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenRouteDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((DeliveryRoute?)null);

        // Act
        var act = async () => await _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── StartRouteAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldStartRoute_WhenStatusIsPlanned()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route = new DeliveryRoute
        {
            Id = id,
            RouteCode = "RT-001",
            BranchId = Guid.NewGuid(),
            Status = RouteStatus.Planned,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Slots = []
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(route);
        _repoMock.Setup(r => r.UpdateAsync(route, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _sut.StartRouteAsync(id);

        // Assert
        route.Status.Should().Be(RouteStatus.InProgress);
        _repoMock.Verify(r => r.UpdateAsync(route, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenStartingNonExistentRoute()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((DeliveryRoute?)null);

        // Act
        var act = async () => await _sut.StartRouteAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── CompleteRouteAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCompleteRoute_WhenStatusIsInProgress()
    {
        // Arrange
        var id = Guid.NewGuid();
        var route = new DeliveryRoute
        {
            Id = id,
            RouteCode = "RT-002",
            BranchId = Guid.NewGuid(),
            Status = RouteStatus.InProgress,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Slots = []
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(route);
        _repoMock.Setup(r => r.UpdateAsync(route, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.CompleteRouteAsync(id);

        // Assert
        route.Status.Should().Be(RouteStatus.Completed);
    }

    // ── GetPagedAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedRoutes_WhenRoutesExist()
    {
        // Arrange
        var routes = new List<DeliveryRoute>
        {
            new() { Id = Guid.NewGuid(), RouteCode = "RT-A", BranchId = Guid.NewGuid(),
                    Status = RouteStatus.Planned, Date = DateOnly.FromDateTime(DateTime.Today), Slots = [] }
        };
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new PagedResult<DeliveryRoute>(routes, 1, 20, 1, 1));

        // Act
        var result = await _sut.GetPagedAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
    }
}
