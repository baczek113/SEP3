namespace WebApp.DTOs.Job;

public class AddJobListingSkillDto
{
    public long JobListingId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string Priority { get; set; } = null!;
}