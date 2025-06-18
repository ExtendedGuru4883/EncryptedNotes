using System.ComponentModel.DataAnnotations;

namespace Client.Models.Forms;

public class SignupFormModel
{
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}