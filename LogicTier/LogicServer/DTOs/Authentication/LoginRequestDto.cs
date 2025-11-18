namespace LogicServer.DTOs.Authentication;

public class LoginRequestDto
{
    public string Email { get; set; } =  null!;
    public string Password { get; set; } = null!;
}