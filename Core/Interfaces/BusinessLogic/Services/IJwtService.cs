namespace Core.Interfaces.BusinessLogic.Services;

public interface IJwtService
{
    string GenerateToken(string username, Guid userId);
}