using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Validations;

namespace Shared.Dto.Requests.User;

public class UpdatePasswordRequest
{
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.SignatureSaltBase64MaxLength, 1)]
    public required string NewSignatureSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.EncryptionSaltBase64MaxLength, 1)]
    public required string NewEncryptionSaltBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.PublicKeyBase64MaxLength, 1)]
    public required string NewPublicKeyBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(AuthConstants.EncryptedEncryptionKeyBase64Maxlength, 1)]
    public required string NewEncryptedEncryptionKeyBase64 { get; init; }
}