using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.Modules.Notification.Domain.Entities;
using SwiftApp.Postal.Modules.Notification.Domain.Enums;
using SwiftApp.Postal.Modules.Notification.Domain.Interfaces;
using SwiftApp.Postal.SharedKernel.Domain;
using Xunit;

namespace SwiftApp.Postal.Modules.Notification.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationLogRepository> _logRepoMock = new();
    private readonly Mock<INotificationTemplateRepository> _templateRepoMock = new();
    private readonly Mock<IInAppNotificationRepository> _inAppRepoMock = new();
    private readonly Mock<INotificationPreferenceRepository> _prefRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<INotificationHubPusher> _hubPusherMock = new();
    private readonly Mock<ILogger<NotificationService>> _loggerMock = new();
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _sut = new NotificationService(
            _logRepoMock.Object,
            _templateRepoMock.Object,
            _inAppRepoMock.Object,
            _prefRepoMock.Object,
            _emailServiceMock.Object,
            _hubPusherMock.Object,
            _loggerMock.Object);
    }

    // ── SendEmailAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldLogAndSendEmail_WhenEmailServiceSucceeds()
    {
        // Arrange
        _logRepoMock.Setup(r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        _logRepoMock.Setup(r => r.UpdateAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        _emailServiceMock.Setup(e => e.SendAsync("test@example.com", "Test", "Subject", "<p>Body</p>", It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

        // Act
        await _sut.SendEmailAsync("test@example.com", "Test", "Subject", "<p>Body</p>", "TestEvent");

        // Assert
        _emailServiceMock.Verify(e => e.SendAsync("test@example.com", "Test", "Subject", "<p>Body</p>", It.IsAny<CancellationToken>()), Times.Once);
        _logRepoMock.Verify(r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _logRepoMock.Verify(r => r.UpdateAsync(
            It.Is<NotificationLog>(l => l.Status == NotificationStatus.Sent),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldMarkLogAsFailed_WhenEmailServiceThrows()
    {
        // Arrange
        _logRepoMock.Setup(r => r.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        _logRepoMock.Setup(r => r.UpdateAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
        _emailServiceMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("SMTP connection refused"));

        // Act
        await _sut.SendEmailAsync("fail@example.com", "Fail", "Subject", "<p>Body</p>", "TestEvent");

        // Assert
        _logRepoMock.Verify(r => r.UpdateAsync(
            It.Is<NotificationLog>(l => l.Status == NotificationStatus.Failed && l.RetryCount == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── SendInAppAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldCreateInAppNotification_AndPushToRole_WhenTargetRoleSet()
    {
        // Arrange
        var request = new InAppNotificationRequest(
            TargetEmployeeId: null,
            TargetRole: "EMPLOYEE",
            Title: "New Parcel",
            Message: "Parcel CHE123 created",
            Category: "Parcel",
            ReferenceUrl: "/app/parcels/1");

        _inAppRepoMock.Setup(r => r.AddAsync(It.IsAny<InAppNotification>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);
        _hubPusherMock.Setup(h => h.PushToRoleAsync(It.IsAny<string>(), It.IsAny<InAppNotificationResponse>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

        // Act
        await _sut.SendInAppAsync(request);

        // Assert
        _inAppRepoMock.Verify(r => r.AddAsync(It.IsAny<InAppNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        _hubPusherMock.Verify(h => h.PushToRoleAsync("EMPLOYEE", It.IsAny<InAppNotificationResponse>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldPushToUser_AndUpdateUnreadCount_WhenTargetEmployeeIdSet()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new InAppNotificationRequest(
            TargetEmployeeId: employeeId,
            TargetRole: null,
            Title: "Personal Notification",
            Message: "For you specifically",
            Category: "Info");

        _inAppRepoMock.Setup(r => r.AddAsync(It.IsAny<InAppNotification>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);
        _inAppRepoMock.Setup(r => r.GetUnreadCountAsync(employeeId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(3);
        _hubPusherMock.Setup(h => h.PushToUserAsync(It.IsAny<string>(), It.IsAny<InAppNotificationResponse>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);
        _hubPusherMock.Setup(h => h.UpdateUnreadCountAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

        // Act
        await _sut.SendInAppAsync(request);

        // Assert
        _hubPusherMock.Verify(h => h.PushToUserAsync(employeeId.ToString(), It.IsAny<InAppNotificationResponse>(), It.IsAny<CancellationToken>()), Times.Once);
        _hubPusherMock.Verify(h => h.UpdateUnreadCountAsync(employeeId.ToString(), 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetLogsPagedAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnPagedLogs_WhenLogsExist()
    {
        // Arrange
        var logs = new List<NotificationLog>
        {
            new() { Id = Guid.NewGuid(), RecipientEmail = "a@b.ch", Type = NotificationType.Email,
                    Status = NotificationStatus.Sent, Subject = "Welcome", EventType = "CustomerCreated" }
        };
        _logRepoMock.Setup(r => r.GetPagedAsync(1, 20, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new PagedResult<NotificationLog>(logs, 1, 20, 1, 1));

        // Act
        var result = await _sut.GetLogsPagedAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].RecipientEmail.Should().Be("a@b.ch");
        result.Items[0].Status.Should().Be("Sent");
    }

    // ── GetAllTemplatesAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task ShouldReturnAllTemplates_WhenTemplatesExist()
    {
        // Arrange
        var templates = new List<NotificationTemplate>
        {
            new() { Id = Guid.NewGuid(), TemplateCode = "customer-welcome", Type = NotificationType.Email,
                    EventType = "CustomerCreated", Translations = [] }
        };
        _templateRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(templates);

        // Act
        var result = await _sut.GetAllTemplatesAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].TemplateCode.Should().Be("customer-welcome");
    }
}
