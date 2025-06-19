using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Shared.Constants;
using Shared.Dto.Requests.Auth.Base;
using Shared.Validations;

namespace Shared.Dto.Requests.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record SignupRequest : BaseAuthRequest
{
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.SignatureSaltBase64MaxLength, 1)]
    public required string SignatureSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.EncryptionSaltBase64MaxLength, 1)]
    public required string EncryptionSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.PublicKeyBase64MaxLength, 1)]
    public required string PublicKeyBase64 { get; init; }
}