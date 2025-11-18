namespace LogicServer.DTOs.Authentication;

public class LoginResponseDto
{
    public String Role  { get; set; }
    public String Name { get; set; }
    public int Id { get; set; }
    public String Email { get; set; }
    public bool Success { get; set; }
}