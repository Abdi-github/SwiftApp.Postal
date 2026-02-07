using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Tracking.Application.DTOs;
using SwiftApp.Postal.Modules.Tracking.Application.Services;

namespace SwiftApp.Postal.Modules.Tracking.Controllers;

[ApiController]
[Route("api/v1/tracking")]
[Produces("application/json")]
public class TrackingController(TrackingService service) : ControllerBase
{
    [HttpGet("{trackingNumber}")]
    [AllowAnonymous]
    [ProducesResponseType<TrackingTimelineResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTimeline(string trackingNumber, CancellationToken ct = default)
    {
        var result = await service.GetTimelineAsync(trackingNumber, ct);
        return Ok(result);
    }

    [HttpPost("{trackingNumber}/events")]
    [Authorize]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RecordEvent(string trackingNumber, [FromBody] TrackingEventRequest request, CancellationToken ct = default)
    {
        var actualRequest = request with { TrackingNumber = trackingNumber };
        await service.RecordEventAsync(actualRequest, User.Identity?.Name, ct);
        return StatusCode(201);
    }
}
