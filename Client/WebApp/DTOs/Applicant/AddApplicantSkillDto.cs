namespace WebApp.DTOs.Applicant;

public class AddApplicantSkillDto
{
    public long ApplicantId { get; set; }      
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }      
    public SkillLevelDto Level { get; set; } 
}