using System.Net.Http.Json;
using System.Text.Json;
using WebApp.DTOs.Application;

namespace WebApp.Services;

public class HttpApplicationService : IApplicationService
{
    private readonly HttpClient client;
    private const string BaseUrl = "api/application";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpApplicationService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<ApplicationDto> CreateAsync(CreateApplicationDto request)
    {
        var httpResponse = await client.PostAsJsonAsync(BaseUrl, request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<ApplicationDto>(response, JsonOptions)!;
    }

    public async Task<List<ApplicationDto>> GetByJobAsync(long jobId)
    {
        var httpResponse = await client.GetAsync($"{BaseUrl}/applications/by-job/{jobId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return ParseResponse(response);
    }

    public async Task<List<ApplicationDto>> GetByApplicantAsync(long applicantId)
    {
        var httpResponse = await client.GetAsync($"{BaseUrl}/applications/by-applicant/{applicantId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return ParseResponse(response);
    }

    private List<ApplicationDto> ParseResponse(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) 
            return new List<ApplicationDto>();

        if (json.TrimStart().StartsWith("["))
        {
            return JsonSerializer.Deserialize<List<ApplicationDto>>(json, JsonOptions) 
                   ?? new List<ApplicationDto>();
        }
        
        var wrapper = JsonSerializer.Deserialize<ApplicationsDto>(json, JsonOptions);
        return wrapper?.Applications ?? new List<ApplicationDto>();
    }

    public async Task AcceptAsync(long applicationId)
    {
        var payload = new { applicationId };
        var httpResponse = await client.PostAsJsonAsync($"{BaseUrl}/accept-application", payload);
        
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to accept: {error}");
        }
    }

    public async Task RejectAsync(long applicationId)
    {
        var payload = new { applicationId };
        var httpResponse = await client.PostAsJsonAsync($"{BaseUrl}/reject-application", payload);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to reject: {error}");
        }
    }
}