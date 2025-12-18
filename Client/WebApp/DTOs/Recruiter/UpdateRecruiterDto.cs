using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.Recruiter;

public class UpdateRecruiterDto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Work email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position / title is required.")]
    public string Position { get; set; } = string.Empty;

    public long WorksInCompanyId { get; set; }
    public long RepresentativeId { get; set; }
}