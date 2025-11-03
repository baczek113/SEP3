using System.Text.Json;
using DTOs;

namespace Services;

public class HttpCompanyRepresentativeService : ICompanyRepresentativeService
{
    private readonly HttpClient client;

    public HttpCompanyRepresentativeService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/representative", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CompanyRepresentativeDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}