namespace WebApp.DTOs.Recruiter;

public class CreateRecruiterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;

    public long HiredByRepresentativeId { get; set; }
    public long WorksInCompanyId { get; set; }
}