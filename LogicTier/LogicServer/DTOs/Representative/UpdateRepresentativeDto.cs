namespace LogicServer.DTOs.Representative;

public class UpdateRepresentativeDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Position { get; set; }
    public string? Password { get; set; }
}