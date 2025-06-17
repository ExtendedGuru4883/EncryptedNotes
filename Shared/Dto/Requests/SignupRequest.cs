using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto.Requests;

public record SignupRequest
{
    [Required]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username must be alphanumeric")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters")]
    public required string Username { get; set; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Signature salt in base64 cannot exceed 256 characters")]
    public required string SignatureSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Encryption salt in base64 cannot exceed 256 characters")]
    public required string EncryptionSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Public key in base64 cannot exceed 256 characters")]
    public required string PublicKeyBase64 { get; set; }
}