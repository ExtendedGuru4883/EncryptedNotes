using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace BusinessLogic.Configurations;

public class JwtSettings()
{
    [Required]
    public required string PrivateKey { get; set; }
    [Range(1, int.MaxValue)]
    public int LifetimeInMinutes { get; set; }
}