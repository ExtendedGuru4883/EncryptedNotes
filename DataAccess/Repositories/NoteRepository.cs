using Core.Abstractions.DataAccess.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class NoteRepository(AppDbContext dbContext) : INoteRepository
{
    public async Task<List<NoteEntity>> GetAllByUserIdAsNoTrackingAsync(Guid userId)
    {
        return await dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }
}