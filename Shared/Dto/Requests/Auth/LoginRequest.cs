using System.ComponentModel.DataAnnotations;
using Shared.Dto.Requests.Auth.Base;
using Shared.Validations;

namespace Shared.Dto.Requests.Auth;

public record LoginRequest : BaseAuthRequest
{
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Nonce in base64 cannot exceed 256 characters")]
    public required string NonceBase64 { get; init; }
    [Required]
    [ValidBase64]
    [MaxLength(256, ErrorMessage = "Nonce signature in base64 cannot exceed 256 characters")]
    public required string NonceSignatureBase64 { get; init; }
}