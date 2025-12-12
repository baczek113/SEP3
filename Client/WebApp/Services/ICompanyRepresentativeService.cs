using DTOs;
using WebApp.DTOs.CompanyRepresentative;

namespace WebApp.Services;

public interface ICompanyRepresentativeService
{
    Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request);
    
    Task<CompanyRepresentativeDto> UpdateCompanyRepresentativeAsync(UpdateCompanyRepresentativeDto request);
    
    Task DeleteCompanyRepresentativeAsync(long id);
}