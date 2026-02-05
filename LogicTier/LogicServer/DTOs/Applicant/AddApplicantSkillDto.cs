namespace LogicServer.DTOs.Applicant;

public class AddApplicantSkillDto
{
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }      // opcjonalne
    public SkillLevelDto Level { get; set; }
}