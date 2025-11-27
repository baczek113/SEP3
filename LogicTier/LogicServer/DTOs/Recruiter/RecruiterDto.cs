namespace LogicServer.DTOs.Recruiter;

public class RecruiterDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;

    public long HiredById { get; set; }
    public long WorksInCompanyId { get; set; }
}