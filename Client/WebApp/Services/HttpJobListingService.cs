using System.Text.Json;
using WebApp.DTOs.Job;

namespace WebApp.Services;

public class HttpJobListingService : IJobListingService
{
    private readonly HttpClient client;

    public HttpJobListingService(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task<JobListingDto> AddJobListing(CreateJobListingDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/joblisting", request);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<JobListingDto>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task<List<JobListingDto>> GetByCompanyAsync(long companyId)
    {
        var httpResponse = await client.GetAsync($"api/joblisting/by-company/{companyId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<JobListingDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task<List<JobListingDto>> GetByRecruiterAsync(long recruiterId)
    {
        var httpResponse = await client.GetAsync($"api/joblisting/by-recruiter/{recruiterId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<JobListingDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task<List<JobListingDto>> GetByCityAsync(string city)
    {
        var httpResponse = await client.GetAsync($"api/joblisting/by-city/{city}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<JobListingDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<List<JobListingDto>> GetSkillsForJobListingAsync(long jobListingId)
    {
        var httpResponse = await client.GetAsync($"api/joblisting/job-skills/by-job-id/{jobListingId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<JobListingDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

}
