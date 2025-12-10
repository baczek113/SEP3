using LogicServer.DTOs.Recruiter;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecruiterController : ControllerBase
{
    private readonly RecruiterService _recruiterService;

    public RecruiterController(RecruiterService recruiterService)
    {
        _recruiterService = recruiterService;
    }
    
    [HttpPost]
    public async Task<ActionResult<RecruiterDto>> CreateRecruiter([FromBody] CreateRecruiterDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        var result = await _recruiterService.CreateRecruiterAsync(dto);
        return Ok(result);
    }
    
    [HttpGet("by-company/{companyId:long}")]
    public async Task<ActionResult<List<RecruiterDto>>> GetRecruitersForCompany(long companyId)
    {
        var result = await _recruiterService.GetRecruitersForCompanyAsync(companyId);
        return Ok(result);
    }
    [HttpGet("{recruiterId:long}")]
    public async Task<ActionResult<RecruiterDto>> GetRecruiterById(long recruiterId)
    {
        var result = await _recruiterService.GetByIdAsync(recruiterId);

        if (result == null)
            return NotFound($"Recruiter with ID {recruiterId} not found.");

        return Ok(result);
    }

    [HttpPut("{recruiterId:long}")]
    public async Task<ActionResult<RecruiterDto>> UpdateRecruiter(long recruiterId, [FromBody] UpdateRecruiterDto dto)
    {
        if (dto == null || dto.Id != recruiterId)
            return BadRequest("Mismatched recruiter id.");

        var result = await _recruiterService.UpdateRecruiterAsync(dto);

        if (result == null)
            return NotFound($"Recruiter with ID {recruiterId} not found or you do not have access.");

        return Ok(result);
    }
    [HttpDelete("{recruiterId:long}")]
    public async Task<IActionResult> RemoveRecruiter(long recruiterId)
    {
        var success = await _recruiterService.RemoveRecruiterAsync(recruiterId);

        if (!success)
            return NotFound($"Recruiter with ID {recruiterId} not found.");

        return NoContent();
    }


}
