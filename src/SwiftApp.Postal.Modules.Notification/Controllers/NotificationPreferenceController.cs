using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Notification.Application.DTOs;
using SwiftApp.Postal.Modules.Notification.Application.Services;

namespace SwiftApp.Postal.Modules.Notification.Controllers;

[ApiController]
[Route("api/v1/notification-preferences")]
[Produces("application/json")]
[Authorize]
public class NotificationPreferenceController(NotificationService service) : ControllerBase
{
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType<NotificationPreferenceResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken ct = default)
    {
        var result = await service.GetPreferenceByCustomerAsync(customerId, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<NotificationPreferenceResponse>(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] NotificationPreferenceRequest request, CancellationToken ct = default)
    {
        var result = await service.CreatePreferenceAsync(request, ct);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId = result.CustomerId }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<NotificationPreferenceResponse>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] NotificationPreferenceRequest request, CancellationToken ct = default)
    {
        var result = await service.UpdatePreferenceAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await service.DeletePreferenceAsync(id, ct);
        return NoContent();
    }
}
