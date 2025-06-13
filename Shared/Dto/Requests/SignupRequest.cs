using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class SignupRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string SignatureSaltBase64 { get; set; }
    [Required]
    public required string EncryptionSaltBase64 { get; set; }
    [Required]
    public required string PublicKeyBase64 { get; set; }
}