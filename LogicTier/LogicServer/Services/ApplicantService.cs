using Grpc.Net.Client;
using System.Security.Cryptography;
using System.Text;
using HireFire.Grpc;                    
using LogicServer.DTOs.Applicant;
using LogicServer.DTOs.JobListing;
using GrpcApplicantService = HireFire.Grpc.ApplicantService;


namespace LogicServer.Services;

public class ApplicantService
{
    private readonly string _grpcAddress;
    private readonly JobListingService _jobListingService;

    public ApplicantService(IConfiguration config, JobListingService jobListingService)
    {
        
        _grpcAddress = config["GrpcSettings:ApplicantServiceUrl"] ?? "http://localhost:9090";
        _jobListingService = jobListingService;
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

    public async Task<List<JobListingDto>> GetSuggestedJobsAsync(long userId)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new GrpcApplicantService.ApplicantServiceClient(channel);
        try
        {
            var request = new GetApplicantRequest()
            {
                Id = userId,
            };

            var applicantResponse = await client.GetApplicantByIdAsync(request);


            List<ApplicantSkillResponse> applicantSkills = await GetApplicantSkillsAsync(userId);
            List<JobListingDto> jobListingsInTheArea =
                await _jobListingService.GetJobListingsByCityAsync(applicantResponse.City);
            Dictionary<JobListingDto, int> jobListingScores = new Dictionary<JobListingDto, int>();

            foreach (var jobListing in jobListingsInTheArea)
            {
                int score = 0;
                List<ApplicantSkillResponse> applicantSkillsMatchedWithJob = new();
                List<JobListingSkillDto> jobListingSkills =
                    await _jobListingService.GetJobListingSkillsAsync(jobListing.Id);
                foreach (JobListingSkillDto skill in jobListingSkills)
                {
                    var matches = applicantSkills.Where(a => a.SkillId == skill.Id).ToList();
                    applicantSkillsMatchedWithJob.AddRange(matches);
                }

                foreach (ApplicantSkillResponse applicantSkill in applicantSkillsMatchedWithJob)
                {
                    string jobListingSkillPriorityString =
                        jobListingSkills.First(skill => skill.Id == applicantSkill.SkillId).Priority;
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

            List<JobListingDto> jobListingsResult = new();

            foreach (var key in jobListingScores.OrderByDescending(kv => kv.Value).Select(kv => kv.Key))
            {
                jobListingsResult.Add(key);
            }

            return jobListingsResult;
        }
        catch (Exception e) {
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
}
