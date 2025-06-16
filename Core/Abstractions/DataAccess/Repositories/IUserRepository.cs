using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<UserEntity> AddAsync(UserEntity user);
    Task<bool> UsernameExists(string username);
    Task<string?> GetSignatureSaltByUsername(string username);
}