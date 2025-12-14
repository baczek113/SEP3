using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
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
    
    public async Task<CompanyDto?> GetCompanyByIdAsync(long companyId)
    {
        var httpResponse = await client.GetAsync($"api/company/{companyId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<CompanyDto>(
            response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
    
    public async Task<List<CompanyDto>> GetMyCompaniesAsync(long representativeId)
    {
        var httpResponse = await client.GetAsync($"api/company/by-representative/{representativeId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<List<CompanyDto>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto request)
    {
        var httpResponse = await client.PutAsJsonAsync($"api/company/{request.Id}", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Status {httpResponse.StatusCode}: {response}");

        return JsonSerializer.Deserialize<CompanyDto>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<List<CompanyDto>> GetCompaniesToApproveAsync()
    {
        var httpResponse = await client.GetAsync($"api/company/to-approve");
        var response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
        return JsonSerializer.Deserialize<List<CompanyDto>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<CompanyDto> ApproveCompany(long companyId)
    {
        var httpResponse = await client.PostAsync($"api/company/{companyId}/approve", null);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<CompanyDto>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    
    public async Task DeleteCompanyAsync(long id)
    {
        HttpResponseMessage httpResponse = await client.DeleteAsync($"api/company/{id}");

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to delete company with ID {id}. Server returned status {httpResponse.StatusCode}.");
    }

}
