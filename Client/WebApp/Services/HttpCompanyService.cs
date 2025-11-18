using System.Text.Json;
using WebApp.DTOs.Company;

namespace WebApp.Services;

public class HttpCompanyService: ICompanyService
{
    private readonly HttpClient client;
    
    public HttpCompanyService(HttpClient client)
        {
        this.client = client;
        }

    public async Task<CompanyDto> AddCompany(CreateCompanyDto request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/Company", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception(response);
        }
        return JsonSerializer.Deserialize<CompanyDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}