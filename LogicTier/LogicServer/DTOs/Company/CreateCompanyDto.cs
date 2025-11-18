namespace LogicServer.DTOs.Company;

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    
    public string City { get; set; } = string.Empty;
    public string? Postcode { get; set; }
    public string? Address { get; set; }
    
    public long CompanyRepresentativeId { get; set; }
}
