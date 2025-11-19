using WebApp.DTOs.Company;

namespace WebApp.Services;

public interface ICompanyService
{
    public Task<CompanyDto> AddCompany(CreateCompanyDto request);
    public Task<List<CompanyDto>> GetMyCompaniesAsync(long representativeId);
}