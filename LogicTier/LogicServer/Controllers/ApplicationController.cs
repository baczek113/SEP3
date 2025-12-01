using Grpc.Core;
using LogicServer.DTOs.Applicant;
using LogicServer.DTOs.Application;
using LogicServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly ApplicationService _service;
    private readonly ILogger<ApplicantController> _logger;

    public ApplicationController(ApplicationService service, ILogger<ApplicantController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Invalid data");
            }

            var result = await _service.CreateApplicationAsync(dto);
            return Ok(result);
        }
        catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
        {
            _logger.LogWarning("Failed to create applicant: {Detail}", e.Status.Detail);
            return Conflict(e.Status.Detail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while creating applicant");
            return StatusCode(500, e.Message);
        }
    }
}