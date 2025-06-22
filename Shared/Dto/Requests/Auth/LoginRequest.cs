using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Dto.Requests.Auth.Base;
using Shared.Validations;

namespace Shared.Dto.Requests.Auth;

public record LoginRequest : BaseAuthRequest
{
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.NonceBase64MaxLength, 1)]
    public required string NonceBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.NonceSignatureBase64MaxLength, 1)]
    public required string NonceSignatureBase64 { get; init; }
}