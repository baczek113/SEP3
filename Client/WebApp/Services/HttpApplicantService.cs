using System.Text.Json;
using WebApp.DTOs.Applicant;
using WebApp.DTOs.Job;

namespace WebApp.Services;

public class HttpApplicantService: IApplicantService
{
    private readonly HttpClient client;

    public HttpApplicantService(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task<ApplicantDto> AddApplicantAsync(AddApplicantDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/applicant", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<ApplicantDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    public async Task<ApplicantSkillDto> AddApplicantSkillAsync(AddApplicantSkillDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/applicant/skills", request);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<ApplicantSkillDto>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<List<ApplicantSkillDto>> GetApplicantSkillsAsync(long applicantId)
    {
        var httpResponse = await client.GetAsync($"api/applicant/skills/by-applicant/{applicantId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<ApplicantSkillDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }


    public async Task<List<JobListingDto>> GetSuggestJobListingAsync(long applicantId)
    {
        var httpResponse = await client.GetAsync($"api/applicant/job-listings/by-applicant/{applicantId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<JobListingDto>>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<ApplicantDto> GetApplicantAsync(long applicantId)
    {
        var httpResponse = await client.GetAsync($"api/applicant/by-id/{applicantId}"); ;
        var response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
        return JsonSerializer.Deserialize<ApplicantDto>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<ApplicantDto> UpdateApplicantAsync(UpdateApplicantDto request)
    {
        var httpResponse = await client.PutAsJsonAsync($"api/applicant/{request.Id}", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<ApplicantDto>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }

}
