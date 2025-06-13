using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Nonce { get; set; }
    [Required]
    public required string NonceSignature { get; set; }
}