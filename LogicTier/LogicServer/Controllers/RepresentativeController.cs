using LogicServer.DTOs.Representative;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepresentativeController : ControllerBase
{
    private readonly RepresentativeService _service;
    
    public RepresentativeController(RepresentativeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<RepresentativeDto>> CreateRepresentative([FromBody] CreateRepresentativeDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Invalid data");
        }

        var result = await _service.CreateRepresentativeAsync(dto);
        return Ok(result);
    }
}