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

    public async Task<bool> UpdateUsernameByIdAsync(Guid id, string newUsername)
    {
        var updated = await dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(u => u.SetProperty(us => us.Username, newUsername));
        
        return updated > 0;
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

    public async Task<bool> DeleteByIdAsync(Guid userId)
    {
        var deleted = await dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }
}