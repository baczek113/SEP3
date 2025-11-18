namespace WebApp.DTOs.Authentication;

public class LoginResponseDto
{
    public string Role { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Success { get; set; }
    public int Id { get; set; }
}