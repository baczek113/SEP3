using DTOs;
using WebApp.DTOs.CompanyRepresentative;

namespace Services;

public interface ICompanyRepresentativeService
{
    public Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request);
}