using Grpc.Core;
using LogicServer.DTOs.Applicant;
using LogicServer.DTOs.JobListing;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("applicant-skills")]
public class ApplicantSkillsController : ControllerBase
{
    private readonly ApplicantService _service;
    private readonly ILogger<ApplicantSkillsController> _logger;

    public ApplicantSkillsController(ApplicantService service, ILogger<ApplicantSkillsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteApplicantSkill(long id)
    {
        if (id <= 0) return BadRequest("Invalid skill id.");

        var result = await _service.RemoveSkillAsync(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

}
