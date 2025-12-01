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
}