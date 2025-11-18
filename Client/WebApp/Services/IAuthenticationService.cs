using WebApp.DTOs.Authentication;

namespace Services;

public interface IAuthenticationService
{
    public Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}