using WebApp.DTOs.Company;

namespace WebApp.Services;

public interface ICompanyService
{
    public Task<CompanyDto> AddCompany(CreateCompanyDto request);
    public Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto request);
    public Task<List<CompanyDto>> GetMyCompaniesAsync(long representativeId);
    
    public Task <List<CompanyDto>> GetCompaniesToApproveAsync();

    public Task<CompanyDto> ApproveCompany(long companyId);
}
