using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Delivery.Application.DTOs;
using SwiftApp.Postal.Modules.Delivery.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Delivery.Controllers;

[ApiController]
[Route("api/v1/deliveries/routes")]
[Produces("application/json")]
[Authorize]
public class DeliveryRouteController(DeliveryRouteService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<DeliveryRouteResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DeliveryRouteResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType<DeliveryRouteResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] DeliveryRouteRequest request, CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/start")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct = default)
    {
        await service.StartRouteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct = default)
    {
        await service.CompleteRouteAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{routeId:guid}/slots/{slotId:guid}/attempts")]
    [ProducesResponseType<DeliveryAttemptResponse>(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RecordAttempt(Guid routeId, Guid slotId, [FromBody] DeliveryAttemptRequest request, CancellationToken ct = default)
    {
        var result = await service.RecordAttemptAsync(routeId, slotId, request, ct);
        return Created(string.Empty, result);
    }
}
