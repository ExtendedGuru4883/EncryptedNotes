using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Client.Models.Forms;

public record AuthFormModel
{
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [CustomStringLength(UserConstants.UsernameMaxLength, 1)]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}