using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class SignupRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string SignatureSalt { get; set; }
    [Required]
    public required string EncryptionSalt { get; set; }
    [Required]
    public required string PublicKey { get; set; }
}