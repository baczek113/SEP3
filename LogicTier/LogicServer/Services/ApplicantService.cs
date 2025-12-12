using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using HireFire.Grpc;                    
using LogicServer.DTOs.Applicant;
using LogicServer.DTOs.Application;
using LogicServer.DTOs.Job;
using LogicServer.DTOs.JobListing;
using GrpcApplicantService = HireFire.Grpc.ApplicantService;


namespace LogicServer.Services;

public class ApplicantService
{
    private readonly string _grpcAddress;
    private readonly JobListingService _jobListingService;
    private readonly ApplicationService _applicationService;

    public ApplicantService(IConfiguration config, JobListingService jobListingService, ApplicationService applicationService)
    {
        
        _grpcAddress = config["GrpcSettings:ApplicantServiceUrl"] ?? "http://localhost:9090";
        _jobListingService = jobListingService;
        _applicationService = applicationService;
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

    public async Task<RemoveApplicantResponseDto> RemoveApplicantAsync(long id)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicantService.ApplicantServiceClient(channel);

        var request = new RemoveApplicantRequest { Id = id };

        try
        {
            var reply = await client.RemoveApplicantAsync(request);

            return new RemoveApplicantResponseDto
            {
                Success = reply.Success,
                Message = reply.Message
            };
        }
        catch (Exception ex)
        {
            return new RemoveApplicantResponseDto
            {
                Success = false,
                Message = $"Error deleting applicant: {ex.Message}"
            };
        }
    }

    public async Task<RemoveApplicantSkillResponseDto> RemoveSkillAsync(long applicantSkillId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new GrpcApplicantService.ApplicantServiceClient(channel);

        var request = new RemoveApplicantSkillRequest
        {
            ApplicantSkillId = applicantSkillId
        };

        var reply = await client.RemoveApplicantSkillAsync(request);

        return new RemoveApplicantSkillResponseDto
        {
            Success = reply.Success,
            Message = reply.Message
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

    public async Task<List<JobListingDto>> GetSuggestedJobsAsync(long userId)
{
    using var channel = GrpcChannel.ForAddress(_grpcAddress);
    var client = new GrpcApplicantService.ApplicantServiceClient(channel);

    try
    {
        var request = new GetApplicantRequest
        {
            Id = userId,
        };

        var applicantResponse = await client.GetApplicantByIdAsync(request);

        var userApplications = await _applicationService.GetApplicationsForApplicantAsync(userId);
        List<ApplicationDto> applications = userApplications.Applications;

        List<ApplicantSkillResponse> applicantSkills = await GetApplicantSkillsAsync(userId);

        List<JobListingDto> jobListingsInTheArea =
            await _jobListingService.GetJobListingsByCityAsync(applicantResponse.City);

        Dictionary<JobListingDto, int> jobListingScores = new();

        foreach (var jobListing in jobListingsInTheArea)
        {
            // jeśli już aplikował na ten job → pomijamy
            if (applications.Any(a => a.JobId == jobListing.Id))
            {
                continue;
            }

            int score = 0;

            List<ApplicantSkillResponse> applicantSkillsMatchedWithJob = new();
            List<JobListingSkillDto> jobListingSkills =
                await _jobListingService.GetJobListingSkillsAsync(jobListing.Id);

            // 🔹 TU: porównujemy po SkillId, nie po Id
            foreach (JobListingSkillDto skill in jobListingSkills)
            {
                var matches = applicantSkills
                    .Where(a => a.SkillId == skill.SkillId)
                    .ToList();

                applicantSkillsMatchedWithJob.AddRange(matches);
            }

            foreach (ApplicantSkillResponse applicantSkill in applicantSkillsMatchedWithJob)
            {
                // 🔹 TU: też porównujemy po SkillId
                string jobListingSkillPriorityString =
                    jobListingSkills
                        .First(skill => skill.SkillId == applicantSkill.SkillId)
                        .Priority;

                int jobListingSkillPriorityInt = 1;
                if (jobListingSkillPriorityString == "must")
                {
                    jobListingSkillPriorityInt = 2;
                }

                score += MapToInt(applicantSkill.Level) * jobListingSkillPriorityInt;
            }

            if (score != 0)
            {
                jobListingScores[jobListing] = score;
            }
        }

        List<JobListingDto> jobListingsResult = jobListingScores
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        return jobListingsResult;
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new List<JobListingDto>();
    }
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
    
    private static int MapToInt(SkillLevelProto proto) =>
        proto switch
        {
            SkillLevelProto.SkillLevelBeginner => 1,
            SkillLevelProto.SkillLevelJunior   => 2,
            SkillLevelProto.SkillLevelMid      => 3,
            SkillLevelProto.SkillLevelSenior   => 4,
            SkillLevelProto.SkillLevelExpert   => 5,
            _                                  => 1
        };
    public async Task<ApplicantDto?> GetByIdAsync(long applicantId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicantService.ApplicantServiceClient(channel);

        var request = new GetApplicantRequest
        {
            Id = applicantId
        };

        var reply = await client.GetApplicantByIdAsync(request);

        if (reply.Id == 0)
            return null;

        return new ApplicantDto
        {
            Id         = reply.Id,
            Name       = reply.Name,
            Email      = reply.Email,
            Experience = reply.Experience,
            City       = reply.City,
            Postcode   = reply.Postcode,
            Address    = reply.Address
        };
    }

    public async Task<ApplicantDto?> UpdateApplicantAsync(UpdateApplicantDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.ApplicantService.ApplicantServiceClient(channel);

        var request = new UpdateApplicantRequest
        {
            Id          = dto.Id,
            Name        = dto.Name,
            Email       = dto.Email,
            Experience  = dto.Experience ?? string.Empty,
            City        = dto.City,
            Postcode    = dto.Postcode ?? string.Empty,
            Address     = dto.Address ?? string.Empty
        };

        var reply = await client.UpdateApplicantAsync(request);

        if (reply.Id == 0)
            return null;

        return new ApplicantDto
        {
            Id         = reply.Id,
            Name       = reply.Name,
            Email      = reply.Email,
            Experience = reply.Experience,
            City       = reply.City,
            Postcode   = reply.Postcode,
            Address    = reply.Address
        };
    }


    
}
