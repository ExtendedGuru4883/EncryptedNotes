using System.ComponentModel.DataAnnotations;
using Shared.Validations;

namespace Shared.Dto.Requests;

public class LoginRequest
{
    [Required]
    public required string Username { get; set; }
    [Required]
    [ValidBase64]
    public required string NonceBase64 { get; set; }
    [Required]
    [ValidBase64]
    public required string NonceSignatureBase64 { get; set; }
}