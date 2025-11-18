using DTOs;
using WebApp.DTOs.CompanyRepresentative;

namespace WebApp.Services;

public interface ICompanyRepresentativeService
{
    public Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request);
}