using System.Security.Claims;
using Core.Abstractions.Infrastructure;

namespace EncryptedNotes.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? Username => httpContextAccessor.HttpContext?.User?.Identity?.Name;
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}