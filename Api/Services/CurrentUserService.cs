using System.Security.Claims;
using Core.Abstractions.Infrastructure;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EncryptedNotes.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? Username => httpContextAccessor.HttpContext?.User.Claims
        .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}