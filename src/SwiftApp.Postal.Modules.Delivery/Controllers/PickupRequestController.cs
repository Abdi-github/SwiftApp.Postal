using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Delivery.Application.DTOs;
using SwiftApp.Postal.Modules.Delivery.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Controllers;

[ApiController]
[Route("api/v1/deliveries/pickups")]
[Produces("application/json")]
[Authorize]
public class PickupRequestController(PickupRequestService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<PickupRequestResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PickupRequestResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<PickupRequestResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] PickupRequestDto request, CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER,EMPLOYEE")]
    [ProducesResponseType<PickupRequestResponse>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> TransitionStatus(Guid id, [FromBody] PickupStatusRequest request, CancellationToken ct = default)
    {
        var result = await service.TransitionStatusAsync(id, request.Status, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct = default)
    {
        await service.CancelAsync(id, ct);
        return NoContent();
    }
}
