using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Address.Application.DTOs;
using SwiftApp.Postal.Modules.Address.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Address.Controllers;

[ApiController]
[Route("api/v1/addresses")]
[Produces("application/json")]
public class SwissAddressController(SwissAddressService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PagedResult<SwissAddressResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType<SwissAddressResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("search/zip/{zipCode}")]
    [AllowAnonymous]
    [ProducesResponseType<List<SwissAddressResponse>>(200)]
    public async Task<IActionResult> SearchByZipCode(string zipCode, CancellationToken ct = default)
    {
        var result = await service.SearchByZipCodeAsync(zipCode, ct);
        return Ok(result);
    }

    [HttpGet("search/canton/{canton}")]
    [AllowAnonymous]
    [ProducesResponseType<List<SwissAddressResponse>>(200)]
    public async Task<IActionResult> SearchByCanton(string canton, CancellationToken ct = default)
    {
        var result = await service.SearchByCantonAsync(canton, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<SwissAddressResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] SwissAddressRequest request, CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<SwissAddressResponse>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] SwissAddressRequest request, CancellationToken ct = default)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
