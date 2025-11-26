using Grpc.Core;
using LogicServer.DTOs.Representative;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepresentativeController : ControllerBase
{
    private readonly RepresentativeService _service;
    private readonly ILogger<RepresentativeController> _logger;

    public RepresentativeController(RepresentativeService service, ILogger<RepresentativeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RepresentativeDto>> CreateRepresentative([FromBody] CreateRepresentativeDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Invalid data");
            }

            var result = await _service.CreateRepresentativeAsync(dto);
            return Ok(result);
        }
        catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
        {
            _logger.LogWarning("Failed to create representative: {Detail} ", e.Status.Detail);
            return Conflict(e.Status.Detail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while creating representative");
            return StatusCode(500, e.Message);
        }
    }
}