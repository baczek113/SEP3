using LogicServer.DTOs.Representative;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepresentativeController : ControllerBase
{
    private readonly RepresentativeService _representativeService;

    public RepresentativeController(RepresentativeService representativeService)
    {
        _representativeService = representativeService;
    }

    [HttpPost]
    public async Task<ActionResult<RepresentativeDto>> CreateRepresentative([FromBody] CreateRepresentativeDto dto)
    {
        Console.WriteLine("LOGIC: HTTP POST /api/representative received - creating company representative");
        if (dto == null)
            return BadRequest("Invalid data");

        var result = await _representativeService.CreateRepresentativeAsync(dto);
        Console.WriteLine("LOGIC: Company representative created in LogicServer, returning response to Client");
        return Ok(result);
        
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<RepresentativeDto>> UpdateRepresentative(long id, [FromBody] UpdateRepresentativeDto dto)
    {
        if (dto == null || dto.Id != id)
            return BadRequest("Mismatched representative id.");

        var result = await _representativeService.UpdateRepresentativeAsync(dto);

        if (result == null)
            return NotFound($"Representative with ID {id} not found.");

        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteRepresentative(long id)
    {
        var success = await _representativeService.DeleteRepresentativeAsync(id);

        if (!success)
            return NotFound($"Representative with ID {id} not found.");

        return NoContent();
    }
}