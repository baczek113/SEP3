using System.Text.Json;
using DTOs;
using WebApp.DTOs.CompanyRepresentative;

namespace WebApp.Services;

public class HttpCompanyRepresentativeService : ICompanyRepresentativeService
{
    private readonly HttpClient client;

    public HttpCompanyRepresentativeService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request)
    {
        Console.WriteLine("CLIENT: Sending CreateCompanyRepresentative request to LogicServer");
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("api/representative", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine("CLIENT: Received response from LogicServer for CreateCompanyRepresentative");
        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<CompanyRepresentativeDto>(
            response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        
    }

    public async Task<CompanyRepresentativeDto> UpdateCompanyRepresentativeAsync(UpdateCompanyRepresentativeDto request)
    {
        HttpResponseMessage httpResponse = await client.PutAsJsonAsync($"api/representative/{request.Id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<CompanyRepresentativeDto>(
            response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task DeleteCompanyRepresentativeAsync(long id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"api/representative/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }
}