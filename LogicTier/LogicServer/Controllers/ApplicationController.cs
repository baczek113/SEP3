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
            _logger.LogWarning("Failed to create application: {Detail}", e.Status.Detail);
            return Conflict(e.Status.Detail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while creating application");
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPost("accept-application")]
    public async Task<ActionResult<ApplicationDto>> AcceptApplication([FromBody] ChangeApplicationStatusDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Invalid data");
            }
    
            var result = await _service.AcceptApplication(dto.ApplicationId);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while changing application status");
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpPost("reject-application")]
    public async Task<ActionResult<ApplicationDto>> RejectApplication([FromBody] ChangeApplicationStatusDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Invalid data");
            }
    
            var result = await _service.RejectApplication(dto.ApplicationId);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while changing application status");
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("applications/by-job/{jobId:long}")]
    public async Task<ActionResult<ApplicationDto>> GetApplicationByJob(long jobId)
    {
        try
        {
            var result = await _service.GetApplicationsForJobAsync(jobId);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while fetching applications");
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("applications/by-applicant/{applicantId:long}")]
    public async Task<ActionResult<ApplicationDto>> GetApplicationByApplicant(long applicantId)
    {
        try
        {
            var result = await _service.GetApplicationsForApplicantAsync(applicantId);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while fetching applications");
            return StatusCode(500, e.Message);
        }
    }
}