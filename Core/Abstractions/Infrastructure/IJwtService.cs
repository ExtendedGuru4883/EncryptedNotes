namespace Core.Abstractions.Infrastructure;

public interface IJwtService
{
    string GenerateToken(string username, Guid userId);
}