using System.ComponentModel.DataAnnotations;

namespace EncryptedNotes.Configurations;

public class JwtSettings
{
    [Required]
    public required string PrivateKey { get; init; }
    [Range(1, int.MaxValue)]
    public int LifetimeInMinutes { get; init; }
    [Required]
    public required string Issuer { get; init; }
    [Required]
    public required string Audience { get; init; }
}