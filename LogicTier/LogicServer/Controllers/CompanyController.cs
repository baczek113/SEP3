using System.Security.Claims;
using LogicServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogicServer.DTOs.Company;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

    public CompanyController(CompanyService companyService)
    {
        _companyService = companyService;
    }
    
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");
        
        var result = await _companyService.CreateCompanyAsync(dto);
        return Ok(result);

    }

    [HttpPut("{companyId:long}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(long companyId, [FromBody] UpdateCompanyDto dto)
    {
        if (dto == null || dto.Id != companyId)
            return BadRequest("Mismatched company id.");

        var result = await _companyService.UpdateCompanyAsync(dto);

        if (result is null)
            return NotFound($"Company with ID {companyId} not found or you do not have access.");

        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyById(long id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);

        if (company == null)
            return NotFound($"Company with id {id} not found");

        return Ok(company);
    }
    
    [HttpGet("by-representative/{representativeId:long}")]
    public async Task<ActionResult<List<CompanyDto>>> GetMyCompanies(long representativeId)
    {
        var result = await _companyService.GetCompaniesForRepresentativeAsync(representativeId);
        return Ok(result);
    }

    [HttpGet("to-approve")]
    public async Task<ActionResult<List<CompanyDto>>> GetCompaniesToApprove()
    {
        var result = await _companyService.GetCompaniesToApproveAsync();
        return Ok(result);
    }

    [HttpPost("{id:long}/approve")]
    public async Task<ActionResult<CompanyDto>> ApproveCompany(long id)
    {
        var result = await _companyService.ApproveCompanyAsync(id);
        return Ok(result);
    }
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCompany(long id)
    {
        var success = await _companyService.RemoveCompanyAsync(id);

        if (!success)
            return NotFound($"Company with ID {id} not found.");

        return NoContent();
    }
}
