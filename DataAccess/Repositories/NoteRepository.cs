using Core.Entities;
using Core.Interfaces.DataAccess.Repositories;
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