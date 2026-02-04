using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Notification.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Produces("application/json")]
[Authorize]
public class NotificationController(NotificationService service) : ControllerBase
{
    [HttpGet("logs")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<PagedResult<NotificationLogResponse>>(200)]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetLogsPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("templates")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<List<NotificationTemplateResponse>>(200)]
    public async Task<IActionResult> GetTemplates(CancellationToken ct = default)
    {
        var result = await service.GetAllTemplatesAsync(ct);
        return Ok(result);
    }

    [HttpGet("inbox/{employeeId:guid}")]
    [ProducesResponseType<List<InAppNotificationResponse>>(200)]
    public async Task<IActionResult> GetInbox(Guid employeeId, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await service.GetInAppForEmployeeAsync(employeeId, limit, ct);
        return Ok(result);
    }

    [HttpGet("inbox/{employeeId:guid}/unread-count")]
    [ProducesResponseType<int>(200)]
    public async Task<IActionResult> GetUnreadCount(Guid employeeId, CancellationToken ct = default)
    {
        var count = await service.GetUnreadInAppCountAsync(employeeId, ct);
        return Ok(count);
    }

    [HttpPost("inbox/{notificationId:guid}/read")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> MarkRead(Guid notificationId, [FromQuery] Guid employeeId, CancellationToken ct = default)
    {
        await service.MarkInAppReadAsync(notificationId, employeeId, ct);
        return NoContent();
    }

    [HttpPost("in-app")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SendInApp([FromBody] InAppNotificationRequest request, CancellationToken ct = default)
    {
        await service.SendInAppAsync(request, ct);
        return Created();
    }

    [HttpPost("email")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request, CancellationToken ct = default)
    {
        await service.SendEmailAsync(request.ToEmail, request.ToName, request.Subject, request.HtmlBody, eventType: null, ct);
        return Created();
    }
}
