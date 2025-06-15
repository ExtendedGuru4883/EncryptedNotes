using Core.Entities;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<UserEntity?> GetByUsernameAsync(string username)
    {
        return await dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<UserEntity> AddAsync(UserEntity user)
    {
        var entityEntry = await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<bool> UsernameExists(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<string?> GetSignatureSaltByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.SignatureSaltBase64)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetEncryptionSaltByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.EncryptionSaltBase64)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetPublicKeyByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.PublicKeyBase64)
            .FirstOrDefaultAsync();
    }
}