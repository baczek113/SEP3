using System.Text.Json;
using WebApp.DTOs.Applicant;

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

}