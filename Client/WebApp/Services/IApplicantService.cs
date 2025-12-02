using WebApp.DTOs.Applicant;
using WebApp.DTOs.Job;

namespace WebApp.Services;

public interface IApplicantService
{
    public Task<ApplicantDto> AddApplicantAsync(AddApplicantDto request);
    public Task<ApplicantSkillDto> AddApplicantSkillAsync(AddApplicantSkillDto request);
    
    public Task <List<ApplicantSkillDto>>  GetApplicantSkillsAsync (long applicantId);
    
    public Task <List<JobListingDto>> GetSuggestJobListingAsync(long applicantId);

}