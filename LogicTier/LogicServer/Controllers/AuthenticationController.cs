using LogicServer.DTOs.Authentication;
using LogicServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _service;
    
    public AuthenticationController(AuthenticationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Invalid data");
        }

        var result = await _service.LoginAsync(dto);
        return result;
    }
}