using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<UserEntity> AddAsync(UserEntity user);
    Task<bool> UpdateUsernameByIdAsync(Guid id, string newUsername);
    Task<bool> UpdatePasswordByIdAsync(Guid id, string newSignatureSaltBase64, string newEncryptionSaltBase64,
        string newPublicKeyBase64, string newEncryptedEncryptionKeyBase64);
    Task<bool> UsernameExistsAsync(string username);
    Task<string?> GetSignatureSaltByUsernameAsync(string username);
    Task<bool> DeleteByIdAsync(Guid userId);
}