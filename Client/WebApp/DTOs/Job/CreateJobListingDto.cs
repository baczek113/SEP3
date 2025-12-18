using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.Job
{
    public class CreateJobListingDto
    {
        [Required(ErrorMessage = "Job title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        // Optional, but if provided it must be positive
        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Salary must be a positive number.")]
        public decimal? Salary { get; set; }
        
        public long CompanyId { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postcode is required.")]
        public string Postcode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
        
        public long PostedById { get; set; }
    }
}