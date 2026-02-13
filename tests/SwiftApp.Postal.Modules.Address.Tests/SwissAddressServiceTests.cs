using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Address.Application.DTOs;
using SwiftApp.Postal.Modules.Address.Application.Services;
using SwiftApp.Postal.Modules.Address.Domain.Entities;
using SwiftApp.Postal.Modules.Address.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Address.Tests;

public class SwissAddressServiceTests
{
    private readonly Mock<ISwissAddressRepository> _repoMock = new();
    private readonly Mock<ILogger<SwissAddressService>> _loggerMock = new();
    private readonly SwissAddressService _sut;

    public SwissAddressServiceTests()
    {
        _sut = new SwissAddressService(_repoMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateAddress_WhenRequestIsValid()
    {
        // Arrange
        var request = new SwissAddressRequest("8001", "Zürich", "ZH", "Zürich Kreis 1");
        _repoMock.Setup(r => r.AddAsync(It.IsAny<SwissAddress>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.ZipCode.Should().Be("8001");
        result.City.Should().Be("Zürich");
        result.Canton.Should().Be("ZH");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<SwissAddress>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnAddress_WhenIdExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var address = new SwissAddress { Id = id, ZipCode = "3000", City = "Bern", Canton = "BE" };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(address);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Id.Should().Be(id);
        result.City.Should().Be("Bern");
        result.Canton.Should().Be("BE");
    }

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenAddressDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((SwissAddress?)null);

        // Act
        var act = async () => await _sut.GetByIdAsync(id);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── SearchByZipCodeAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnAddresses_MatchingZipCode()
    {
        // Arrange
        var addresses = new List<SwissAddress>
        {
            new() { Id = Guid.NewGuid(), ZipCode = "1200", City = "Genève", Canton = "GE" },
            new() { Id = Guid.NewGuid(), ZipCode = "1200", City = "Genève 1", Canton = "GE" }
        };
        _repoMock.Setup(r => r.GetByZipCodeAsync("1200", It.IsAny<CancellationToken>())).ReturnsAsync(addresses);

        // Act
        var result = await _sut.SearchByZipCodeAsync("1200");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.ZipCode.Should().Be("1200"));
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenNoAddressMatchesZipCode()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByZipCodeAsync("9999", It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        // Act
        var result = await _sut.SearchByZipCodeAsync("9999");

        // Assert
        result.Should().BeEmpty();
    }

    // ── SearchByCantonAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnAddresses_MatchingCanton()
    {
        // Arrange
        var addresses = new List<SwissAddress>
        {
            new() { Id = Guid.NewGuid(), ZipCode = "6000", City = "Luzern", Canton = "LU" },
            new() { Id = Guid.NewGuid(), ZipCode = "6010", City = "Kriens", Canton = "LU" }
        };
        _repoMock.Setup(r => r.GetByCantonAsync("LU", It.IsAny<CancellationToken>())).ReturnsAsync(addresses);

        // Act
        var result = await _sut.SearchByCantonAsync("LU");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.Canton.Should().Be("LU"));
    }

    // ── GetPagedAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedResult_WhenAddressesExist()
    {
        // Arrange
        var addresses = new List<SwissAddress>
        {
            new() { Id = Guid.NewGuid(), ZipCode = "4000", City = "Basel", Canton = "BS" }
        };
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new PagedResult<SwissAddress>(addresses, 1, 20, 1, 1));

        // Act
        var result = await _sut.GetPagedAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalItems.Should().Be(1);
        result.Items[0].City.Should().Be("Basel");
    }
}
