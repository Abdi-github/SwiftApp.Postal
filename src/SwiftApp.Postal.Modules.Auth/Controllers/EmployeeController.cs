using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Auth.Application.DTOs;
using SwiftApp.Postal.Modules.Auth.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Auth.Controllers;

[ApiController]
[Route("api/v1/employees")]
[Produces("application/json")]
[Authorize]
public class EmployeeController(EmployeeService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType<PagedResult<EmployeeResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType<EmployeeResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<EmployeeResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request, CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType<EmployeeResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeRequest request, CancellationToken ct = default)
    {
        var result = await service.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("sync")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<EmployeeResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SyncKeycloak([FromBody] EmployeeSyncRequest request, CancellationToken ct = default)
    {
        var result = await service.SyncKeycloakUserAsync(request, ct);
        return Ok(result);
    }
}
