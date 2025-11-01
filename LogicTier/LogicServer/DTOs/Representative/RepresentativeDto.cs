namespace LogicServer.DTOs.Representative;

public class RepresentativeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Position { get; set; } = null!;
}