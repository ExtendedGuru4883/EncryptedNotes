using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Shared.Dto;

public record UserDto
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [CustomStringLength(UserConstants.UsernameMaxLength)]
    public required string Username { get; set; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.SignatureSaltBase64MaxLength, 1)]
    public required string SignatureSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.EncryptionSaltBase64MaxLength, 1)]
    public required string EncryptionSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.PublicKeyBase64MaxLength, 1)]
    public required string PublicKeyBase64 { get; set; }
}