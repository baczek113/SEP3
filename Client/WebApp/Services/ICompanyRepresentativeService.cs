using DTOs;

namespace Services;

public interface ICompanyRepresentativeService
{
    public Task<CompanyRepresentativeDto> AddCompanyRepresentativeAsync(AddCompanyRepresentativeDto request);
}