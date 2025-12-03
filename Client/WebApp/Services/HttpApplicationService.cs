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

        return JsonSerializer.Deserialize<List<ApplicationDto>>(response, JsonOptions)!;
    }



    public async Task<List<ApplicationDto>> GetByApplicantAsync(long applicantId)
    {
        var httpResponse = await client.GetAsync($"{BaseUrl}/applications/by-applicant/{applicantId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<ApplicationDto>>(response, JsonOptions)!;
    }

    public async Task AcceptAsync(long applicationId)
    {
        var payload = new { applicationId };

        var httpResponse = await client.PostAsJsonAsync(
            $"{BaseUrl}/accept-application",
            payload);

        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }

    public async Task RejectAsync(long applicationId)
    {
        var payload = new { applicationId };

        var httpResponse = await client.PostAsJsonAsync(
            $"{BaseUrl}/reject-application",
            payload);

        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }
}
