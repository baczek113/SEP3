using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.Applicant
{
    public class UpdateApplicantDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Experience is required.")]
        public string Experience { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postcode is required.")]
        public string Postcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
    }
}