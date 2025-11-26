namespace WebApp.DTOs.Job

{
    public class CreateJobListingDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Salary { get; set; }

        public long CompanyId { get; set; }
        public string City { get; set; } = null!;
        
        public string Postcode { get; set; } = null!;
        
        public string Address { get; set; } = null!;
        
        public long PostedById { get; set; }
    }
}