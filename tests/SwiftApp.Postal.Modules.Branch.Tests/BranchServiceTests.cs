using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Branch.Application.DTOs;
using SwiftApp.Postal.Modules.Branch.Application.Services;
using SwiftApp.Postal.Modules.Branch.Domain.Entities;
using SwiftApp.Postal.Modules.Branch.Domain.Enums;
using SwiftApp.Postal.Modules.Branch.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using SwiftApp.Postal.SharedKernel.Exceptions;
using Xunit;

namespace SwiftApp.Postal.Modules.Branch.Tests;

public class BranchServiceTests
{
    private readonly Mock<IBranchRepository> _repoMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<BranchService>> _loggerMock = new();
    private readonly BranchService _sut;

    public BranchServiceTests()
    {
        _sut = new BranchService(_repoMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateBranch_WhenRequestIsValid()
    {
        // Arrange
        var request = new BranchRequest(
            "ZRH-001", "PostOffice", "Bahnhofstrasse 1", "8001", "Zürich",
            "ZH", "+41 44 000 00 00", "zrh001@swisspost.ch", 47.376m, 8.547m,
            [new BranchTranslationRequest("de-CH", "Zürich Hauptbahnhof", null)]);

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Branch>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateAsync(request, "de-CH");

        // Assert
        result.Id.Should().NotBeEmpty();
        result.BranchCode.Should().Be("ZRH-001");
        result.Status.Should().Be("Active");
        result.City.Should().Be("Zürich");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Branch>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnLocalisedName_WhenLocaleMatches()
    {
        // Arrange
        var id = Guid.NewGuid();
        var branch = new Domain.Entities.Branch
        {
            Id = id,
            BranchCode = "GVA-001",
            Type = BranchType.PostOffice,
            Status = BranchStatus.Active,
            Street = "Rue du Mont-Blanc 1",
            ZipCode = "1201",
            City = "Genève",
            Translations =
            [
                new BranchTranslation { Locale = "de", Name = "Genf Hauptpost", Description = null },
                new BranchTranslation { Locale = "fr", Name = "Genève Bureau Principal", Description = "Bureau principal" }
            ]
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(branch);

        // Act
        var result = await _sut.GetByIdAsync(id, "fr-CH");

        // Assert
        result.Name.Should().Be("Genève Bureau Principal");
        result.Description.Should().Be("Bureau principal");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldThrowEntityNotFoundException_WhenBranchDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Branch?)null);

        // Act
        var act = async () => await _sut.GetByIdAsync(id, "de-CH");

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    // ── GetPagedAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedResult_WhenBranchesExist()
    {
        // Arrange
        var branches = new List<Domain.Entities.Branch>
        {
            new() { Id = Guid.NewGuid(), BranchCode = "A", Type = BranchType.PostOffice, Status = BranchStatus.Active,
                    Street = "S", ZipCode = "1000", City = "Lausanne", Translations = [] },
            new() { Id = Guid.NewGuid(), BranchCode = "B", Type = BranchType.PostOffice, Status = BranchStatus.Active,
                    Street = "S", ZipCode = "3000", City = "Bern", Translations = [] }
        };
        _repoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new PagedResult<Domain.Entities.Branch>(branches, 1, 20, 2, 1));

        // Act
        var result = await _sut.GetPagedAsync(1, 20, "de-CH");

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldDeleteBranch_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var branch = new Domain.Entities.Branch
        {
            Id = id,
            BranchCode = "TMP-001",
            Type = BranchType.PostOffice,
            Status = BranchStatus.Active,
            Street = "S", ZipCode = "9000", City = "St. Gallen",
            Translations = []
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(branch);
        _repoMock.Setup(r => r.SoftDeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteAsync(id);

        // Assert
        _repoMock.Verify(r => r.SoftDeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
