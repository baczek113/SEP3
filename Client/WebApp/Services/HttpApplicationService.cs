using WebApp.DTOs.Application;

public class HttpApplicationService : IApplicationService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "api/Application";

    public HttpApplicationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApplicationDto> CreateAsync(CreateApplicationDto dto)
    {
        var response = await _http.PostAsJsonAsync(BaseUrl, dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ApplicationDto>()
               ?? throw new InvalidOperationException("Empty response body");
    }

    public async Task<ICollection<ApplicationDto>> GetByJobAsync(long jobId)
    {
        var result =
            await _http.GetFromJsonAsync<ICollection<ApplicationDto>>(
                $"{BaseUrl}/applications/by-job/{jobId}");

        return result ?? Array.Empty<ApplicationDto>();
    }

    public async Task<ICollection<ApplicationDto>> GetByApplicantAsync(long applicantId)
    {
        var result =
            await _http.GetFromJsonAsync<ICollection<ApplicationDto>>(
                $"{BaseUrl}/applications/by-applicant/{applicantId}");

        return result ?? Array.Empty<ApplicationDto>();
    }

    public async Task AcceptAsync(long applicationId)
    {
        var response = await _http.PostAsJsonAsync(
            $"{BaseUrl}/accept-application",
            new { applicationId });

        response.EnsureSuccessStatusCode();
    }

    public async Task RejectAsync(long applicationId)
    {
        var response = await _http.PostAsJsonAsync(
            $"{BaseUrl}/reject-application",
            new { applicationId });

        response.EnsureSuccessStatusCode();
    }
}