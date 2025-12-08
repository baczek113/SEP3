namespace LogicServer.DTOs.Recruiter;

public class UpdateRecruiterDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public long WorksInCompanyId { get; set; }
    public long RepresentativeId { get; set; }
}
