using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<UserEntity> AddAsync(UserEntity user);
    Task<bool> UsernameExistsAsync(string username);
    Task<string?> GetSignatureSaltByUsernameAsync(string username);
    Task<bool> DeleteByIdAsync(Guid userId);
}