using System.ComponentModel.DataAnnotations;

namespace LogicServer.DTOs.Company;

public class UpdateCompanyDto
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;
    public string? Postcode { get; set; }
    public string? Address { get; set; }

    public long CompanyRepresentativeId { get; set; }
}
