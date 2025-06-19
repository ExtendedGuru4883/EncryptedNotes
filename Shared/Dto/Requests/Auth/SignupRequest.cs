using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Shared.Dto.Requests.Auth.Base;
using Shared.Validations;

namespace Shared.Dto.Requests.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record SignupRequest : BaseAuthRequest
{
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Signature salt in base64 cannot exceed 256 characters")]
    public required string SignatureSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Encryption salt in base64 cannot exceed 256 characters")]
    public required string EncryptionSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Public key in base64 cannot exceed 256 characters")]
    public required string PublicKeyBase64 { get; init; }
}