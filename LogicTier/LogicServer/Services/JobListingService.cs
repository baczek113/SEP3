using System.Collections;
using System.Globalization;
using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.Job;
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
            PostedById  = dto.PostedById,
            IsClosed    = false
        };

        var reply = await client.CreateJobListingAsync(request);

        return MapToDto(reply);
    }

    public async Task<RemoveJobListingResponseDto> RemoveJobListingAsync(RemoveJobListingRequestDto requestDto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new RemoveJobListingRequest
        {
            Id = requestDto.Id
        };
        
        var reply = await client.RemoveJobListingAsync(request);

        return new RemoveJobListingResponseDto
        {
            Success = reply.Success,
            Message = reply.Message
        };
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

    public async Task<List<JobListingSkillDto>> GetJobListingSkillsAsync(long jobId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new GetJobListingSkillsRequest
        {
            JobListingId = jobId
        };

        var reply = await client.GetJobListingSkillsAsync(request);

        var skills = new List<JobListingSkillDto>();

        foreach (var skill in reply.Skills)
        {
            skills.Add(new JobListingSkillDto
            {
                Id           = skill.Id,            
                Priority     = skill.Priority,      
                JobListingId = skill.JobListingId,
                SkillId      = skill.SkillId,
                SkillName    = skill.SkillName
            });
        }

        return skills;
    }
    public async Task<List<JobListingDto>> GetJobListingsByCityAsync(string city)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);
    
        var request = new GetJobListingsByCityRequest()
        {
            CityName = city
        };
    
        var reply = await client.GetJobListingsByCityAsync(request);

        return reply.Listings
            .Select(MapToDto)
            .Where(j => !j.IsClosed)
            .ToList();
    }

    public async Task<JobListingDto> UpdateJobListingAsync(UpdateJobListingDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new UpdateJobListingRequest
        {
            Id          = dto.Id,
            Title       = dto.Title,
            Description = dto.Description ?? string.Empty,
            Salary      = dto.Salary.HasValue ? dto.Salary.Value.ToString("0.##", CultureInfo.InvariantCulture) : string.Empty,
            City        = dto.City,
            Postcode    = dto.Postcode ?? string.Empty,
            Address     = dto.Address ?? string.Empty,
            CompanyId   = dto.CompanyId,
            IsClosed    = dto.IsClosed
        };

        var reply = await client.UpdateJobListingAsync(request);
        return MapToDto(reply);
    }

    public async Task<JobListingDto> CloseJobListingAsync(long jobListingId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new CloseJobListingRequest
        {
            Id = jobListingId
        };

        var reply = await client.CloseJobListingAsync(request);
        return MapToDto(reply);
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
            PostedById = postedById,
            IsClosed   = reply.IsClosed
        };
    }
    public async Task<JobListingSkillDto> AddJobListingSkillAsync(AddJobListingSkillDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        if (string.IsNullOrWhiteSpace(dto.SkillName))
            throw new ArgumentException("SkillName must not be empty.");

        if (string.IsNullOrWhiteSpace(dto.Priority))
            throw new ArgumentException("Priority must be 'must' or 'nice'.");

        var request = new AddJobListingSkillRequest
        {
            JobListingId = dto.JobListingId,
            SkillName    = dto.SkillName,
            Category     = dto.Category ?? string.Empty,
            Priority     = dto.Priority      
        };

        var reply = await client.AddJobListingSkillAsync(request);

        
        return new JobListingSkillDto
        {
            Id           = reply.Id,
            Priority     = reply.Priority,
            JobListingId = reply.JobListingId,
            SkillId      = reply.SkillId,
            SkillName    = reply.SkillName
        };
    }
    public async Task<JobListingDto?> GetJobListingByIdAsync(long jobId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new GetJobListingByIdRequest
        {
            Id = jobId
        };

        try
        {
            var reply = await client.GetJobListingByIdAsync(request);
            return MapToDto(reply);
        }
        catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return null;
        }
    }

    

    public async Task<RemoveJobListingSkillResponseDto> RemoveJobListingSkillAsync(long jobListingSkillId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.JobListingService.JobListingServiceClient(channel);

        var request = new RemoveJobListingSkillRequest
        {
            JobListingSkillId = jobListingSkillId
        };

        var reply = await client.RemoveJobListingSkillAsync(request);

        return new RemoveJobListingSkillResponseDto
        {
            Success = reply.Success,
            Message = reply.Message
        };
    }

}
