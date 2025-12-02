using WebApp.DTOs.Job;

namespace WebApp.Services;

public interface IJobListingService
{
    public Task<JobListingDto> AddJobListing(CreateJobListingDto request);
    
    public Task<List<JobListingDto>> GetByCompanyAsync(long recruiterId);
    
    public Task<List<JobListingDto>> GetByRecruiterAsync(long companyId);
    
    public Task<List<JobListingDto>> GetByCityAsync (String city);
    
    public Task<List<JobListingSkillDto>> GetSkillsForJobListingAsync(long jobListingId);

    public Task<JobListingSkillDto> AddSkillsForJobListing(AddJobListingSkillDto request);

}