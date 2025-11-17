using WebApp.DTOs.Applicant;

namespace Services;

public interface IApplicantService
{
    public Task<ApplicantDto> AddApplicantAsync(AddApplicantDto request);
    public Task<ApplicantSkillDto> AddApplicantSkillAsync(AddApplicantSkillDto request);

}