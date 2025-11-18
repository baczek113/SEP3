using WebApp.DTOs.Company;

namespace WebApp.Services;

public interface ICompanyService
{
    public Task<CompanyDto> AddCompany(CreateCompanyDto request);
}