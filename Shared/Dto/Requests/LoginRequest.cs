using System.ComponentModel.DataAnnotations;

namespace Shared.Dto.Requests;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string NonceBase64 { get; set; }
    [Required]
    public required string NonceSignatureBase64 { get; set; }
}