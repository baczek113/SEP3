using Grpc.Net.Client;
using HireFire.Grpc;
using LogicServer.DTOs.Company;

namespace LogicServer.Services;

public class CompanyService
{
    private readonly string _grpcAddress;

    public CompanyService(IConfiguration config)
    {
        _grpcAddress = config["GrpcSettings:CompanyServiceUrl"] ?? "http://localhost:9090";
    }

    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto)
    {
        using var channel = GrpcChannel.ForAddress(_grpcAddress);
        var client = new HireFire.Grpc.CompanyService.CompanyServiceClient(channel);

        var request = new CreateCompanyRequest
        {
            Name        = dto.Name,
            Description = dto.Description ?? string.Empty,
            Website     = dto.Website ?? string.Empty,

            City        = dto.City,
            Postcode    = dto.Postcode ?? string.Empty,
            Address     = dto.Address ?? string.Empty,

            CompanyRepresentativeId = dto.CompanyRepresentativeId
        };

        var reply = await client.CreateCompanyAsync(request);

        return new CompanyDto
        {
            Id                     = reply.Id,
            Name                   = reply.Name,
            Description            = string.IsNullOrWhiteSpace(reply.Description) ? null : reply.Description,
            Website                = string.IsNullOrWhiteSpace(reply.Website) ? null : reply.Website,
            IsApproved             = reply.IsApproved,
            City                   = reply.City,
            Postcode               = string.IsNullOrWhiteSpace(reply.Postcode) ? null : reply.Postcode,
            Address                = string.IsNullOrWhiteSpace(reply.Address) ? null : reply.Address,
            CompanyRepresentativeId = reply.CompanyRepresentativeId
        };
    }
     public async Task<List<CompanyDto>> GetCompaniesForRepresentativeAsync(long representativeId)
     {
         using var channel = GrpcChannel.ForAddress(_grpcAddress);
         var client = new HireFire.Grpc.CompanyService.CompanyServiceClient(channel);
     
         var request = new GetCompaniesForRepresentativeRequest
         {
             CompanyRepresentativeId = representativeId
         };
     
         var reply = await client.GetCompaniesForRepresentativeAsync(request);
         
         return reply.Companies
             .Select(c => new CompanyDto
             {
                 Id                      = c.Id,
                 Name                    = c.Name,
                 Description             = string.IsNullOrWhiteSpace(c.Description) ? null : c.Description,
                 Website                 = string.IsNullOrWhiteSpace(c.Website) ? null : c.Website,
                 IsApproved              = c.IsApproved,
                 City                    = c.City,
                 Postcode                = string.IsNullOrWhiteSpace(c.Postcode) ? null : c.Postcode,
                 Address                 = string.IsNullOrWhiteSpace(c.Address) ? null : c.Address,
                 CompanyRepresentativeId = c.CompanyRepresentativeId
             })
             .ToList();
     }
}
