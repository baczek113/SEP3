namespace WebApp.DTOs.Application;

public class ApplicationDto
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long ApplicantId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; }
}