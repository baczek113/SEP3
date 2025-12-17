using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.CompanyRepresentative;

public class UpdateCompanyRepresentativeDto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required.")]
    public string Position { get; set; } = string.Empty;

    public string? Password { get; set; }
}