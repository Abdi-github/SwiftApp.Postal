using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Auth.Application.DTOs;
using SwiftApp.Postal.Modules.Auth.Application.Services;
using SwiftApp.Postal.Modules.Auth.Domain.Entities;
using SwiftApp.Postal.Modules.Auth.Domain.Enums;
using SwiftApp.Postal.Modules.Auth.Domain.Events;
using SwiftApp.Postal.Modules.Auth.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Auth.Tests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<EmployeeService>> _loggerMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(_repoMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateEmployee_WhenRequestIsValid()
    {
        // Arrange
        var request = new EmployeeRequest("Jane", "Doe", "jane@postal.ch", null, "Employee", null, DateOnly.FromDateTime(DateTime.Today), "de-CH");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("jane@postal.ch");
        result.Role.Should().Be("Employee");
        result.Status.Should().Be("Active");
        result.EmployeeNumber.Should().StartWith("EMP-");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldPublishEmployeeCreatedEvent_WhenCreated()
    {
        // Arrange
        var request = new EmployeeRequest("Jane", "Doe", "jane@postal.ch", null, "Employee", null, DateOnly.FromDateTime(DateTime.Today), "de-CH");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        await _sut.CreateAsync(request);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.Is<EmployeeCreatedEvent>(e => e.Email == "jane@postal.ch"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnEmployee_WhenIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employee = BuildEmployee(id, "emp@postal.ch");
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(employee);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Id.Should().Be(id);
        result.Email.Should().Be("emp@postal.ch");
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenIdDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Employee?)null);

        // Act
        var act = () => _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldUpdateEmployee_WhenEmployeeExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employee = BuildEmployee(id, "old@postal.ch");
        var request = new EmployeeRequest("Updated", "Name", "new@postal.ch", "+41001", "BranchManager", null, DateOnly.FromDateTime(DateTime.Today), "fr-CH");

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAsync(id, request);

        // Assert
        result.FirstName.Should().Be("Updated");
        result.Email.Should().Be("new@postal.ch");
        result.Role.Should().Be("BranchManager");
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenUpdatingNonExistentEmployee()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        // Act
        var act = () => _sut.UpdateAsync(id, new EmployeeRequest("A", "B", "c@d.ch", null, "Employee", null, DateOnly.FromDateTime(DateTime.Today), "de-CH"));

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── GetPagedAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedResult_WithMappedResponses()
    {
        // Arrange
        var employees = Enumerable.Range(1, 3).Select(i => BuildEmployee(Guid.NewGuid(), $"emp{i}@postal.ch")).ToList();
        var pagedResult = new PagedResult<Employee>(employees, 1, 20, 3, 1);
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>())).ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetPagedAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(1);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCallSoftDelete_WhenDeleteCalled()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.SoftDeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        _repoMock.Verify(r => r.SoftDeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Employee BuildEmployee(Guid id, string email) => new()
    {
        Id = id,
        EmployeeNumber = "EMP-20260101-ABCD",
        FirstName = "Test",
        LastName = "User",
        Email = email,
        Role = EmployeeRole.Employee,
        Status = EmployeeStatus.Active,
        HireDate = DateOnly.FromDateTime(DateTime.Today),
        PreferredLocale = "de-CH",
        CreatedAt = DateTimeOffset.UtcNow
    };
}
