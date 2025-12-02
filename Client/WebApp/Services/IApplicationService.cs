using WebApp.DTOs.Application;

public interface IApplicationService
{
    Task<ApplicationDto> CreateAsync(CreateApplicationDto dto);

    Task<ICollection<ApplicationDto>> GetByJobAsync(long jobId);
    Task<ICollection<ApplicationDto>> GetByApplicantAsync(long applicantId);

    Task AcceptAsync(long applicationId);
    Task RejectAsync(long applicationId);
}