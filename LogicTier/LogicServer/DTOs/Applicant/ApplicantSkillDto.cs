namespace LogicServer.DTOs.Applicant;

public class ApplicantSkillDto
{
    public long Id { get; set; }              // id rekordu applicant_skill
    public long ApplicantId { get; set; }
    public long SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public SkillLevelDto Level { get; set; }
}