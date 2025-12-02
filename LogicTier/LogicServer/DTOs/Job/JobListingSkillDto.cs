namespace LogicServer.DTOs.Job;

public class JobListingSkillDto
{
    public long Id { get; set; }

    public string Priority { get; set; } = null!;
    
    public long JobListingId { get; set; }
    
    public long SkillId { get; set; }
    
    public string SkillName { get; set; } = string.Empty;
}