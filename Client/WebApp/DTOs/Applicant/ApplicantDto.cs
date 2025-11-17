namespace WebApp.DTOs.Applicant;

public class ApplicantDto
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}