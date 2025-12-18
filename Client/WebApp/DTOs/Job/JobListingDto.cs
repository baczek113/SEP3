using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.Job

{
    public class JobListingDto
    {
        public long Id { get; set; }

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime DatePosted { get; set; }
        
        public decimal? Salary { get; set; }

        public long CompanyId { get; set; }
        public string City { get; set; } = null!;
        
        public string Postcode { get; set; } = null!;
        
        public string Address { get; set; } = null!;
        public long? PostedById { get; set; }
        public bool IsClosed { get; set; }
    }
}
