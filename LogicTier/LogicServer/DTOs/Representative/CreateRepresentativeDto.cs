namespace LogicServer.DTOs.Representative;

public class CreateRepresentativeDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Position { get; set; } = null!;
}