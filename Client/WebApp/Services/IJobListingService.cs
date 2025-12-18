using WebApp.DTOs.Job;

namespace WebApp.Services;

public interface IJobListingService
{
    public Task<JobListingDto> AddJobListing(CreateJobListingDto request);
    
    public Task<RemoveJobListingResponseDto> DeleteJobListingAsync(long jobListingId);
    
    public Task<List<JobListingDto>> GetByCompanyAsync(long recruiterId);
    
    public Task<List<JobListingDto>> GetByRecruiterAsync(long companyId);
    
    public Task<List<JobListingDto>> GetByCityAsync (String city);
    
    public Task<List<JobListingSkillDto>> GetSkillsForJobListingAsync(long jobListingId);

    public Task<JobListingSkillDto> AddSkillsForJobListing(AddJobListingSkillDto request);

    public Task<RemoveJobListingSkillResponseDto> RemoveJobListingSkillAsync(long jobListingSkillId);

    public Task<JobListingDto?> GetJobListingByIdAsync(long jobListingId);

    public Task<JobListingDto> UpdateJobListingAsync(UpdateJobListingDto request);

    public Task<JobListingDto> CloseJobListingAsync(long jobListingId);
    public Task<JobListingDto> GetByIdAsync(long jobListingId);

}
