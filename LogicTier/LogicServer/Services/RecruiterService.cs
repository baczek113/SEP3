using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.Recruiter;

namespace LogicServer.Services;

public class RecruiterService
{
    private readonly string _grpcAddress;

    public RecruiterService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:RecruiterServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<RecruiterDto> CreateRecruiterAsync(CreateRecruiterDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RecruiterService.RecruiterServiceClient(channel);
        var passwordHash = AuthenticationService.HashPassword(dto.Password);

        var request = new RegisterRecruiterRequest
        {
            Email                    = dto.Email,
            PasswordHash                = passwordHash,
            Name                     = dto.Name,
            Position                 = dto.Position ?? string.Empty,
            HiredByRepresentativeId  = dto.HiredByRepresentativeId,
            WorksInCompanyId         = dto.WorksInCompanyId
        };

        var reply = await client.RegisterRecruiterAsync(request);

        return new RecruiterDto
        {
            Id               = reply.Id,
            Email            = reply.Email,
            Name             = reply.Name,
            Position         = string.IsNullOrWhiteSpace(reply.Position) ? string.Empty : reply.Position,
            HiredById        = reply.HiredById,
            WorksInCompanyId = reply.WorksInCompanyId
        };
    }

    public async Task<List<RecruiterDto>> GetRecruitersForCompanyAsync(long companyId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RecruiterService.RecruiterServiceClient(channel);

        var request = new GetRecruitersForCompanyRequest
        {
            CompanyId = companyId
        };

        var reply = await client.GetRecruitersForCompanyAsync(request);

        return reply.Recruiters
            .Select(r => new RecruiterDto
            {
                Id               = r.Id,
                Email            = r.Email,
                Name             = r.Name,
                Position         = string.IsNullOrWhiteSpace(r.Position) ? string.Empty : r.Position,
                HiredById        = r.HiredById,
                WorksInCompanyId = r.WorksInCompanyId
            })
            .ToList();
    }
    public async Task<RecruiterDto?> GetByIdAsync(long recruiterId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RecruiterService.RecruiterServiceClient(channel);

        var request = new GetRecruiterByIdRequest
        {
            Id = recruiterId
        };

        var reply = await client.GetRecruiterByIdAsync(request);

        // recruiter not found → Java returns id = 0
        if (reply.Id == 0)
            return null;

        return new RecruiterDto
        {
            Id = reply.Id,
            Email = reply.Email,
            Name = reply.Name,
            Position = reply.Position,
            HiredById = reply.HiredById,
            WorksInCompanyId = reply.WorksInCompanyId
        };
    }

    public async Task<RecruiterDto?> UpdateRecruiterAsync(UpdateRecruiterDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RecruiterService.RecruiterServiceClient(channel);

        var passwordHash = string.IsNullOrWhiteSpace(dto.Password)
            ? string.Empty
            : AuthenticationService.HashPassword(dto.Password);

        var request = new UpdateRecruiterRequest
        {
            RecruiterId = dto.Id,
            Email = dto.Email ?? string.Empty,
            Name = dto.Name ?? string.Empty,
            PasswordHash = passwordHash,
            Position = dto.Position ?? string.Empty,
            WorksInCompanyId = dto.WorksInCompanyId,
            RepresentativeId = dto.RepresentativeId
        };

        var reply = await client.UpdateRecruiterAsync(request);

        if (reply.Id == 0)
        {
            return null;
        }

        return new RecruiterDto
        {
            Id = reply.Id,
            Email = reply.Email,
            Name = reply.Name,
            Position = reply.Position,
            HiredById = reply.HiredById,
            WorksInCompanyId = reply.WorksInCompanyId
        };
    }
    public async Task<bool> RemoveRecruiterAsync(long recruiterId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.RecruiterService.RecruiterServiceClient(channel);

        var request = new RemoveRecruiterRequest
        {
            Id = recruiterId
        };

        var reply = await client.RemoveRecruiterAsync(request);

        return reply.Success;
    }

}
