using System.ComponentModel.DataAnnotations;

namespace Client.Models;

public class SignupFormModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}