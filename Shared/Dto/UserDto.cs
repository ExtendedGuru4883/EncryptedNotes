using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto;

public class UserDto
{
    [Required]
    public required string Username { get; set; }
    [Required]
    [ValidBase64]
    public required string SignatureSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    public required string EncryptionSaltBase64 { get; set; }
    [Required]
    [ValidBase64]
    public required string PublicKeyBase64 { get; set; }
}