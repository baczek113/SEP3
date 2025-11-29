using Grpc.Core;
using LogicServer.DTOs.Applicant;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicantController : ControllerBase
{
    private readonly ApplicantService _service;
    private readonly ILogger<ApplicantController> _logger;

    public ApplicantController(ApplicantService service, ILogger<ApplicantController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicantDto>> CreateApplicant([FromBody] CreateApplicantDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Invalid data");
            }

            var result = await _service.CreateApplicantAsync(dto);
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
    
    [HttpPost("skills")]
    public async Task<ActionResult<ApplicantSkillDto>> AddSkill([FromBody] AddApplicantSkillDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        try
        {
            var result = await _service.AddSkillAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("skills/by-applicant/{applicantId:long}")]
    public async Task<ActionResult<List<ApplicantSkillDto>>> GetApplicantSkills(long applicantId)
    {
        var result = await _service.GetApplicantSkillsAsync(applicantId);
        return Ok(result);
    }
    
}