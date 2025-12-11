namespace LogicServer.DTOs.Applicant;

public class UpdateApplicantDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Experience { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Postcode { get; set; } = null!;
    public string Address { get; set; } = null!;
}
