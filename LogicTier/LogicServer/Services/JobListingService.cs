using System.Globalization;
using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.JobListing;

namespace LogicServer.Services;

public class JobListingService
{
    private readonly string _grpcAddress;

    public JobListingService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:JobListingServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<JobListingDto> CreateJobListingAsync(CreateJobListingDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new CreateJobListingRequest
        {
            Title       = dto.Title,
            Description = dto.Description ?? string.Empty,
            Salary      = dto.Salary.HasValue 
                ? dto.Salary.Value.ToString("0.##", CultureInfo.InvariantCulture) 
                : string.Empty,
            CompanyId   = dto.CompanyId,
            City          = dto.City,
            Postcode      = dto.Postcode,
            Address       = dto.Address,
            PostedById  = dto.PostedById
        };

        var reply = await client.CreateJobListingAsync(request);

        return MapToDto(reply);
    }
    
    public async Task<List<JobListingDto>> GetJobListingsForCompanyAsync(long companyId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new GetJobListingsForCompanyRequest
        {
            CompanyId = companyId
        };

        var reply = await client.GetJobListingsForCompanyAsync(request);

        return reply.JobListings
            .Select(MapToDto)
            .ToList();
    }

    
    public async Task<List<JobListingDto>> GetJobListingsForRecruiterAsync(long recruiterId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new GetJobListingsForRecruiterRequest
        {
            RecruiterId = recruiterId
        };

        var reply = await client.GetJobListingsForRecruiterAsync(request);

        return reply.JobListings
            .Select(MapToDto)
            .ToList();
    }
    
    private JobListingDto MapToDto(JobListingResponse reply)
    {
        DateTime datePosted;
        if (!DateTime.TryParse(
                reply.DatePosted,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out datePosted))
        {
            datePosted = DateTime.UtcNow;
        }

        decimal? salary = null;
        if (!string.IsNullOrWhiteSpace(reply.Salary) &&
            decimal.TryParse(reply.Salary, NumberStyles.Any, CultureInfo.InvariantCulture, out var s))
        {
            salary = s;
        }

        long? postedById = reply.PostedById == 0 ? null : reply.PostedById;

        return new JobListingDto
        {
            Id         = reply.Id,
            Title      = reply.Title,
            Description = string.IsNullOrWhiteSpace(reply.Description) ? null : reply.Description,
            DatePosted = datePosted,
            Salary     = salary,
            CompanyId  = reply.CompanyId,
            City          = reply.City,
            Postcode      = reply.Postcode,
            Address       = reply.Address,
            PostedById = postedById
        };
    }
}
