using Core.Entities;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
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
            .FirstOrDefaultAsync(u => u.Username == username) != null;
    }

    public async Task<string?> GetSignatureSaltByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.SignatureSalt)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetEncryptionSaltByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.EncryptionSalt)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetPublicKeyByUsername(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.PublicKey)
            .FirstOrDefaultAsync();
    }
}