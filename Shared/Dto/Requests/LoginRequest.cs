using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto.Requests;

public record LoginRequest
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters")]
    public required string Username { get; set; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Nonce in base64 cannot exceed 256 characters")]
    public required string NonceBase64 { get; set; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Nonce signature in base64 cannot exceed 256 characters")]
    public required string NonceSignatureBase64 { get; set; }
}