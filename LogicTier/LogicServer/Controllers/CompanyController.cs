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
    
    [HttpGet("by-representative/{representativeId:long}")]
    public async Task<ActionResult<List<CompanyDto>>> GetMyCompanies(long representativeId)
    {
        var result = await _companyService.GetCompaniesForRepresentativeAsync(representativeId);
        return Ok(result);
    }
}