using LogicServer.DTOs.Job;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;
using LogicServer.DTOs.JobListing;
using JobListingService = LogicServer.Services.JobListingService;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobListingController : ControllerBase
{
    private readonly JobListingService _jobListingService;

    public JobListingController(JobListingService jobListingService)
    {
        _jobListingService = jobListingService;
    }
    
    [HttpPost]
    public async Task<ActionResult<JobListingDto>> CreateJobListing([FromBody] CreateJobListingDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        var result = await _jobListingService.CreateJobListingAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteJobListing(long id)
    {
        var dto = new RemoveJobListingRequestDto
        {
            Id = id
        };

        var result = await _jobListingService.RemoveJobListingAsync(dto);

        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(new { message = result.Message });
    }


    [HttpGet("by-company/{companyId:long}")]
    public async Task<ActionResult<List<JobListingDto>>> GetByCompany(long companyId)
    {
        var result = await _jobListingService.GetJobListingsForCompanyAsync(companyId);
        return Ok(result);
    }

    [HttpGet("by-recruiter/{recruiterId:long}")]
    public async Task<ActionResult<List<JobListingDto>>> GetByRecruiter(long recruiterId)
    {
        var result = await _jobListingService.GetJobListingsForRecruiterAsync(recruiterId);
        return Ok(result);
    }
    
    [HttpGet("by-city/{city}")]
    public async Task<ActionResult<List<JobListingDto>>> GetByCity(string city)
    {
        var result = await _jobListingService.GetJobListingsByCityAsync(city);
        return Ok(result);
    }
    
    [HttpGet("job-skills/by-job-id/{jobId:long}")]
    public async Task<ActionResult<List<JobListingSkillDto>>> GetSkillsForJob(long jobId)
    {
        var result = await _jobListingService.GetJobListingSkillsAsync(jobId);
        return Ok(result);
    }
    [HttpPost("{jobId:long}/skills")]
    public async Task<ActionResult<JobListingSkillDto>> AddSkillToJob(long jobId, [FromBody] AddJobListingSkillDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        dto.JobListingId = jobId;

        var result = await _jobListingService.AddJobListingSkillAsync(dto);
        return Ok(result);
    }

    [HttpGet("{jobId:long}")]
    public async Task<ActionResult<JobListingDto>> GetJobListingById(long jobId)
    {
        var result = await _jobListingService.GetJobListingByIdAsync(jobId);
        if (result == null)
            return NotFound($"Job listing with id {jobId} not found.");

        return Ok(result);
    }

    [HttpPut("{jobId:long}")]
    public async Task<ActionResult<JobListingDto>> UpdateJobListing(long jobId, [FromBody] UpdateJobListingDto dto)
    {
        if (dto == null || dto.Id != jobId)
            return BadRequest("Mismatched job listing id.");

        var updated = await _jobListingService.UpdateJobListingAsync(dto);
        return Ok(updated);
    }

    [HttpPost("{jobId:long}/close")]
    public async Task<ActionResult<JobListingDto>> CloseJobListing(long jobId)
    {
        var updated = await _jobListingService.CloseJobListingAsync(jobId);
        return Ok(updated);
    }

}
