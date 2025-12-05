using WebApp.DTOs.Job;

namespace WebApp.Services;

public interface IJobListingService
{
    public Task<JobListingDto> AddJobListing(CreateJobListingDto request);
    
    public Task<RemoveJobListingResponseDto> DeleteJobListingAsync(long jobListingId);
    
    public Task<List<JobListingDto>> GetByCompanyAsync(long recruiterId);
    
    public Task<List<JobListingDto>> GetByRecruiterAsync(long companyId);
}