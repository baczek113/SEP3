namespace LogicServer.DTOs.Authentication;

public class LoginResponseDto
{
    public String Role  { get; set; } = null!;
    public String Name { get; set; } = null!;
    public bool Success { get; set; }
}