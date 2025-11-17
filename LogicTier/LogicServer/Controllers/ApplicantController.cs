using LogicServer.DTOs.Applicant;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicantController : ControllerBase
{
    private readonly ApplicantService _service;

    public ApplicantController(ApplicantService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ApplicantDto>> CreateApplicant([FromBody] CreateApplicantDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Invalid data");
        }

        var result = await _service.CreateApplicantAsync(dto);
        return Ok(result);
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

    
    
}