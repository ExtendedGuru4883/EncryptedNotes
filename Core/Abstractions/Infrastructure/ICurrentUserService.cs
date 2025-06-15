namespace Core.Abstractions.Infrastructure;

public interface ICurrentUserService
{
    string? Username { get; }
    string? UserId { get; }
}