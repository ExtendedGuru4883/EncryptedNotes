using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests.Auth.Base;

public record BaseAuthRequest
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters")]
    public required string Username { get; init; }
}