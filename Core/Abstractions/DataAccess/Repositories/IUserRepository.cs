using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByUsernameAsNoTrackingAsync(string username);
    Task<UserEntity> AddAsync(UserEntity user);
    Task<bool> UsernameExists(string username);
    Task<string?> GetSignatureSaltByUsername(string username);
    Task<string?> GetEncryptionSaltByUsername(string username);
    Task<string?> GetPublicKeyByUsername(string username);
}