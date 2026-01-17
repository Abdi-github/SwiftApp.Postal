using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Parcel.Application.DTOs;
using SwiftApp.Postal.Modules.Parcel.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Parcel.Controllers;

[ApiController]
[Route("api/v1/parcels")]
[Produces("application/json")]
[Authorize]
public class ParcelController(ParcelService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ParcelResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ParcelResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("tracking/{trackingNumber}")]
    [AllowAnonymous]
    [ProducesResponseType<ParcelResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByTrackingNumber(string trackingNumber, CancellationToken ct = default)
    {
        var result = await service.GetByTrackingNumberAsync(trackingNumber, ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<ParcelResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] ParcelRequest request, CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ParcelResponse>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ParcelRequest request, CancellationToken ct = default)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER,EMPLOYEE")]
    [ProducesResponseType<ParcelResponse>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> TransitionStatus(Guid id, [FromBody] ParcelStatusRequest request, CancellationToken ct = default)
    {
        var result = await service.TransitionStatusAsync(id, request.Status, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct = default)
    {
        await service.CancelAsync(id, ct);
        return NoContent();
    }
}
