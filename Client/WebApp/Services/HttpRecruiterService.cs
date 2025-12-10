using System.Text.Json;
using WebApp.DTOs.Recruiter;

namespace WebApp.Services;

public class HttpRecruiterService : IRecruiterService
{
    private readonly HttpClient client;

    public HttpRecruiterService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<RecruiterDto> AddRecruiter(CreateRecruiterDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/Recruiter", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
    
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Status {httpResponse.StatusCode}: {response}");
            
        }

        return JsonSerializer.Deserialize<RecruiterDto>(response, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<RecruiterDto>> GetRecruitersForCompanyAsync(long companyId)
    {
        var httpResponse = await client.GetAsync($"api/recruiter/by-company/{companyId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<RecruiterDto>>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<RecruiterDto> GetRecruiterByIdAsync(long recruiterId)
    {
        var httpResponse = await client.GetAsync($"api/recruiter/{recruiterId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Status {httpResponse.StatusCode}: {response}");
        }
        return JsonSerializer.Deserialize<RecruiterDto>(response, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<RecruiterDto> UpdateRecruiterAsync(UpdateRecruiterDto request)
    {
        var httpResponse = await client.PutAsJsonAsync($"api/recruiter/{request.Id}", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Status {httpResponse.StatusCode}: {response}");
        }

        return JsonSerializer.Deserialize<RecruiterDto>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    public async Task DeleteRecruiterAsync(long recruiterId)
    {
        var httpResponse = await client.DeleteAsync($"api/recruiter/{recruiterId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Status {httpResponse.StatusCode}: {response}");
        }
    }
    
}
