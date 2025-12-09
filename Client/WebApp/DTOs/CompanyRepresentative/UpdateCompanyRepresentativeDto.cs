namespace WebApp.DTOs.CompanyRepresentative;

public class UpdateCompanyRepresentativeDto
{
    
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Position { get; set; }
    public string? Password { get; set; }

}