namespace Core.Interfaces.Infrastructure;

public interface IJwtService
{
    string GenerateToken(string username, Guid userId);
}