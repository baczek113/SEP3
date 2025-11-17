namespace WebApp.DTOs.Company;

public class CompanyDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public bool IsApproved { get; set; }
    
    public string City { get; set; } = string.Empty;
    public string? Postcode { get; set; }
    public string? Address { get; set; }

    public long CompanyRepresentativeId { get; set; }
}