using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiftApp.Postal.Modules.Branch.Application.DTOs;
using SwiftApp.Postal.Modules.Branch.Application.Services;
using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.Modules.Branch.Controllers;

[ApiController]
[Route("api/v1/branches")]
[Produces("application/json")]
[Authorize]
public class BranchController(BranchService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<BranchResponse>>(200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string locale = "de", CancellationToken ct = default)
    {
        var result = await service.GetPagedAsync(page, size, locale, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<BranchResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] string locale = "de", CancellationToken ct = default)
    {
        var result = await service.GetByIdAsync(id, locale, ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType<BranchResponse>(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] BranchRequest request, [FromQuery] string locale = "de", CancellationToken ct = default)
    {
        var result = await service.CreateAsync(request, locale, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ADMIN,BRANCH_MANAGER")]
    [ProducesResponseType<BranchResponse>(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] BranchRequest request, [FromQuery] string locale = "de", CancellationToken ct = default)
    {
        var result = await service.UpdateAsync(id, request, locale, ct);
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
}
