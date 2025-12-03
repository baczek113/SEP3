using WebApp.DTOs.Application;

public interface IApplicationService
{
    Task<ApplicationDto> CreateAsync(CreateApplicationDto dto);

    Task<List<ApplicationDto>> GetByJobAsync(long jobId);
    Task<List<ApplicationDto>> GetByApplicantAsync(long applicantId);

    Task AcceptAsync(long applicationId);
    Task RejectAsync(long applicationId);
}