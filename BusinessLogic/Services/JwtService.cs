using System.Security.Claims;
using System.Text;
using BusinessLogic.Configurations;
using Core.Interfaces.BusinessLogic.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly SymmetricSecurityKey _key =
        new(Encoding.UTF8.GetBytes(jwtSettings.Value.PrivateKey));

    private readonly int _lifetimeInMinutes = jwtSettings.Value.LifetimeInMinutes;

    public string GenerateToken(string username)
    {
        var claims = new Dictionary<string, object>()
        {
            [ClaimTypes.Name] = username,
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_lifetimeInMinutes),
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
        };
        
        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}