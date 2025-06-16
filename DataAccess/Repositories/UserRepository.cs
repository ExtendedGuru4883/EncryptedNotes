using Core.Abstractions.DataAccess.Repositories;
using Core.Entities;
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

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<string?> GetSignatureSaltByUsernameAsync(string username)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => u.SignatureSaltBase64)
            .FirstOrDefaultAsync();
    }
}