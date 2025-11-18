using WebApp.DTOs.Applicant;

namespace WebApp.Services;

public interface IApplicantService
{
    public Task<ApplicantDto> AddApplicantAsync(AddApplicantDto request);
    public Task<ApplicantSkillDto> AddApplicantSkillAsync(AddApplicantSkillDto request);

}