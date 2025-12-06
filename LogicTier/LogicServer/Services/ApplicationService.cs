using System.Globalization;
using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.Application;

namespace LogicServer.Services;

public class ApplicationService
{
    private readonly string _grpcAddress;

    public ApplicationService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:ApplicantServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<ApplicationDto> CreateApplicationAsync(CreateApplicationDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicationService.ApplicationServiceClient(channel);
    
        var request = new CreateApplicationRequest()
        {
            ApplicantId = dto.ApplicantId,
            JobId = dto.JobId
        };
    
        var reply = await client.CreateApplicationAsync(request);
    
        DateTime dateSubmitted;
        if (!DateTime.TryParse(
                reply.SubmittedAt,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out dateSubmitted))
        {
            dateSubmitted = DateTime.UtcNow;
        }
        
        return new ApplicationDto()
        {
            Id = reply.Id,
            ApplicantId = reply.ApplicantId,
            JobId = reply.JobId,
            SubmittedAt = dateSubmitted,
            Status = reply.Status.ToString(),
        };
    }
    
    public async Task<List<ApplicationDto>> GetApplicationsForJobAsync(long jobId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicationService.ApplicationServiceClient(channel);

        var request = new GetApplicationsForJobRequest()
        {
            JobId = jobId
        };

        var reply = await client.GetApplicationsForJobAsync(request);

        List<ApplicationDto> applications = new();

        foreach (var application in reply.Applications)
        {
            // poprawny parse SubmittedAt
            if (!DateTime.TryParse(
                    application.SubmittedAt,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var dateSubmitted))
            {
                dateSubmitted = DateTime.UtcNow;
            }

            // KONWERSJA STATUSU ENUM DO STRINGA
            string status = application.Status.ToString();

            // 🔥 OBSŁUGA WSZYSTKICH MOŻLIWYCH FORMATÓW
            bool isUnderReview =
                status.Equals("under_review", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("UnderReview", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("APPLICATION_STATUS_UNDER_REVIEW", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("0"); // czasem enum = 0 → under review

            applications.Add(new ApplicationDto()
            {
                Id = application.Id,
                ApplicantId = application.ApplicantId,
                JobId = application.JobId,
                SubmittedAt = dateSubmitted,
                Status = status
            });
        }

        return applications;
    }


    public async Task<ApplicationsDto> GetApplicationsForApplicantAsync(long applicantId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicationService.ApplicationServiceClient(channel);
    
        var request = new GetApplicationsForApplicantRequest()
        {
            ApplicantId = applicantId
        };
    
        var reply = await client.GetApplicationsForApplicantAsync(request);
        
        List<ApplicationDto> applications = new List<ApplicationDto>();
        
        foreach (var application in reply.Applications)
        {
            DateTime dateSubmitted;
            if (!DateTime.TryParse(
                    application.SubmittedAt,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out dateSubmitted))
            {
                dateSubmitted = DateTime.UtcNow;
            }
            
            applications.Add(new  ApplicationDto()
            {
                Id = application.Id,
                ApplicantId = application.ApplicantId,
                JobId = application.JobId,
                SubmittedAt = dateSubmitted,
                Status = application.Status.ToString(),
            });
        }
        
        
        return new ApplicationsDto()
        {
            Applications = applications
        };
    }

    public async Task<ApplicationDto> AcceptApplication(long applicationId)
    {
        return await UpdateApplicationStatusHelperAsync(true, applicationId);
    }
    
    public async Task<ApplicationDto> RejectApplication(long applicationId)
    {
        return await UpdateApplicationStatusHelperAsync(false, applicationId);
    }

    private async Task<ApplicationDto> UpdateApplicationStatusHelperAsync(bool accept, long applicationId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicationService.ApplicationServiceClient(channel);
    
        var request = new ChangeStatusRequest()
        {
            ApplicationId = applicationId
        };
        
        ApplicationResponse reply;
        if (accept)
        {
            reply = await client.AcceptApplicationAsync(request);
        }
        else
        {
            reply = await client.RejectApplicationAsync(request);
        }

        DateTime dateSubmitted;
        if (!DateTime.TryParse(
                reply.SubmittedAt,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out dateSubmitted))
        {
            dateSubmitted = DateTime.UtcNow;
        }
        
        return new ApplicationDto()
        {
            Id = reply.Id,
            ApplicantId = reply.ApplicantId,
            JobId = reply.JobId,
            SubmittedAt = dateSubmitted,
            Status = reply.Status.ToString(),
        };
    }
}