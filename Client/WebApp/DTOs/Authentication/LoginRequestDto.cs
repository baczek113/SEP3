using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs.Authentication;

public class LoginRequestDto
{
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}