using WebApp.DTOs.Applicant;
using WebApp.DTOs.Job;

namespace WebApp.Services;

public interface IApplicantService
{
    public Task<ApplicantDto> AddApplicantAsync(AddApplicantDto request);
    public Task<ApplicantSkillDto> AddApplicantSkillAsync(AddApplicantSkillDto request);
    
    public Task <List<ApplicantSkillDto>>  GetApplicantSkillsAsync (long applicantId);
    
    public Task<RemoveApplicantSkillResponseDto> RemoveApplicantSkillAsync(long applicantSkillId);
    
    public Task <List<JobListingDto>> GetSuggestJobListingAsync(long applicantId);
    
    public Task <ApplicantDto> GetApplicantAsync(long applicantId);
    
    public Task<ApplicantDto> UpdateApplicantAsync(UpdateApplicantDto request);

}
