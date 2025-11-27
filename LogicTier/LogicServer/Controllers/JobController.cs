using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;
using LogicServer.DTOs.JobListing;

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
}