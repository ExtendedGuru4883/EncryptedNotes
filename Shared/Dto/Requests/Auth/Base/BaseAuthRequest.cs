using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Shared.Dto.Requests.Auth.Base;

public record BaseAuthRequest
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [CustomStringLength(UserConstants.UsernameMaxLength, 1)]
    public required string Username { get; init; }
}