using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using HireFire.Grpc;                    
using LogicServer.DTOs.Applicant;
using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.Applicant;
using HireFire.Grpc;
using GrpcApplicantService = HireFire.Grpc.ApplicantService;


namespace LogicServer.Services;

public class ApplicantService
{
    private readonly string _grpcAddress;

    public ApplicantService(IConfiguration config)
    {
        
        _grpcAddress = config["GrpcSettings:ApplicantServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<ApplicantDto> CreateApplicantAsync(CreateApplicantDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        
        var client = new HireFire.Grpc.ApplicantService.ApplicantServiceClient(channel);

        
        var passwordHash = HashPassword(dto.Password);
        
        var request = new CreateApplicantRequest
        {
            Name          = dto.Name,
            Email         = dto.Email,
            PasswordHash  = passwordHash,
            Experience    = dto.Experience,
            City          = dto.City,
            Postcode      = dto.Postcode,
            Address       = dto.Address
        };

        
        var reply = await client.CreateApplicantAsync(request);
        
        return new ApplicantDto
        {
            Id          = reply.Id,
            Name        = reply.Name,
            Email       = reply.Email,
            Experience  = reply.Experience,
            City        = reply.City,
            Postcode    = reply.Postcode,
            Address     = reply.Address
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
    public async Task<ApplicantSkillDto> AddSkillAsync(AddApplicantSkillDto dto)
    {
    
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new GrpcApplicantService.ApplicantServiceClient(channel);


        var request = new AddApplicantSkillRequest
        {
            ApplicantId = dto.ApplicantId,
            SkillName = dto.SkillName,
            Category = dto.Category ?? string.Empty,
            Level = MapToProto(dto.Level)
        };

        var reply = await client.AddApplicantSkillAsync(request);

    
        return new ApplicantSkillDto
        {
            Id = reply.Id,
            ApplicantId = reply.ApplicantId,
            SkillId = reply.SkillId,
            SkillName = reply.SkillName,
            Level = MapToDto(reply.Level)
        };
    }

    public async Task<List<ApplicantSkillResponse>> GetApplicantSkillsAsync(long userId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new GrpcApplicantService.ApplicantServiceClient(channel);
    
    
        var request = new GetApplicantSkillsRequest()
        {
            ApplicantId = userId,
        };
    
        var reply = await client.GetApplicantSkillsAsync(request);

        List<ApplicantSkillResponse> response = new();

        foreach (var applicantSkill in reply.Skills)
        {
            response.Add(applicantSkill);
        }

        return response;
    }
    
    private static SkillLevelProto MapToProto(SkillLevelDto level) =>
        level switch
        {
            SkillLevelDto.Beginner => SkillLevelProto.SkillLevelBeginner,
            SkillLevelDto.Junior   => SkillLevelProto.SkillLevelJunior,
            SkillLevelDto.Mid      => SkillLevelProto.SkillLevelMid,
            SkillLevelDto.Senior   => SkillLevelProto.SkillLevelSenior,
            SkillLevelDto.Expert   => SkillLevelProto.SkillLevelExpert,
            _                      => SkillLevelProto.SkillLevelBeginner
        };

    private static SkillLevelDto MapToDto(SkillLevelProto proto) =>
        proto switch
        {
            SkillLevelProto.SkillLevelBeginner => SkillLevelDto.Beginner,
            SkillLevelProto.SkillLevelJunior   => SkillLevelDto.Junior,
            SkillLevelProto.SkillLevelMid      => SkillLevelDto.Mid,
            SkillLevelProto.SkillLevelSenior   => SkillLevelDto.Senior,
            SkillLevelProto.SkillLevelExpert   => SkillLevelDto.Expert,
            _                                  => SkillLevelDto.Beginner
        };


    
}
