namespace LogicServer.DTOs.Applicant;

public class ApplicantSkillDto
{
    public long Id { get; set; }
    public long ApplicantId { get; set; }
    public long SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public SkillLevelDto Level { get; set; }
}