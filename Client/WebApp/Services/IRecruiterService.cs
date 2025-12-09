using WebApp.DTOs.Recruiter;

namespace WebApp.Services;

public interface IRecruiterService
{
    Task<RecruiterDto> AddRecruiter(CreateRecruiterDto request);
    Task<List<RecruiterDto>> GetRecruitersForCompanyAsync(long companyId);
    
    Task<RecruiterDto> GetRecruiterByIdAsync(long recruiterId);
    Task<RecruiterDto> UpdateRecruiterAsync(UpdateRecruiterDto request);
    
    
}
