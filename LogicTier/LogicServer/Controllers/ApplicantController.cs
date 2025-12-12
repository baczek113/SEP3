using Grpc.Core;
using LogicServer.DTOs.Applicant;
using LogicServer.DTOs.JobListing;
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

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteApplicant(long id)
    {
        var result = await _service.RemoveApplicantAsync(id);

        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(new { message = result.Message });
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
    
    [HttpDelete("skills/{applicantSkillId:long}")]
    public async Task<ActionResult<RemoveApplicantSkillResponseDto>> RemoveSkill(long applicantSkillId)
    {
        if (applicantSkillId <= 0) return BadRequest("Invalid skill id.");

        var result = await _service.RemoveSkillAsync(applicantSkillId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpGet("skills/by-applicant/{applicantId:long}")]
    public async Task<ActionResult<List<ApplicantSkillDto>>> GetApplicantSkills(long applicantId)
    {
        var result = await _service.GetApplicantSkillsAsync(applicantId);
        var mapped = result.Select(MapToDto).ToList();
        return Ok(mapped);
    }
    
    [HttpGet("job-listings/by-applicant/{applicantId:long}")]
    public async Task<ActionResult<List<JobListingDto>>> GetSuggestedJobListings(long applicantId)
    {
        var result = await _service.GetSuggestedJobsAsync(applicantId);
        return Ok(result);
    }
    [HttpGet("by-id/{applicantId:long}")]
    public async Task<ActionResult<ApplicantDto>> GetApplicationsById(long applicantId)
    {
        var result = await _service.GetByIdAsync(applicantId);
        return Ok(result);
    }

    [HttpPut("{applicantId:long}")]
    public async Task<ActionResult<ApplicantDto>> UpdateApplicant(long applicantId, [FromBody] UpdateApplicantDto dto)
    {
        if (dto == null || dto.Id != applicantId)
            return BadRequest("Invalid applicant payload.");

        try
        {
            var updated = await _service.UpdateApplicantAsync(dto);

            if (updated == null)
                return NotFound($"Applicant with ID {applicantId} not found.");

            return Ok(updated);
        }
        catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.AlreadyExists)
        {
            _logger.LogWarning("Failed to update applicant: {Detail}", e.Status.Detail);
            return Conflict(e.Status.Detail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal Server Error while updating applicant");
            return StatusCode(500, e.Message);
        }
    }

    private static ApplicantSkillDto MapToDto(HireFire.Grpc.ApplicantSkillResponse proto)
    {
        return new ApplicantSkillDto
        {
            Id = proto.Id,
            ApplicantId = proto.ApplicantId,
            SkillId = proto.SkillId,
            SkillName = proto.SkillName,
            Level = proto.Level switch
            {
                HireFire.Grpc.SkillLevelProto.SkillLevelBeginner => SkillLevelDto.Beginner,
                HireFire.Grpc.SkillLevelProto.SkillLevelJunior   => SkillLevelDto.Junior,
                HireFire.Grpc.SkillLevelProto.SkillLevelMid      => SkillLevelDto.Mid,
                HireFire.Grpc.SkillLevelProto.SkillLevelSenior   => SkillLevelDto.Senior,
                HireFire.Grpc.SkillLevelProto.SkillLevelExpert   => SkillLevelDto.Expert,
                _                                                => SkillLevelDto.Beginner
            }
        };
    }
}
