using System.Text.Json;
using WebApp.DTOs.Applicant;
using WebApp.DTOs.Authentication;

namespace Services;

public class HttpAuthenticationService: IAuthenticationService
{
    private readonly HttpClient client;

    public HttpAuthenticationService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/authentication", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<LoginResponseDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}