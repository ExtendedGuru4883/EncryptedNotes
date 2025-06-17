using Core.Abstractions.DataAccess.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class NoteRepository(AppDbContext dbContext) : INoteRepository
{
    public async Task<List<NoteEntity>> GetAllByUserIdAsync(Guid userId)
    {
        return await dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }

    public async Task<(List<NoteEntity> notes, int totalCount)> GetPageByUserIdAsync(Guid userId, int page, int pageSize)
    {
        var filterQuery = dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId);
        
        var notesTask = filterQuery
            .OrderByDescending(n => n.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var totalCountTask = filterQuery
            .CountAsync();
        
        await Task.WhenAll(notesTask, totalCountTask);
        
        return (notesTask.Result, totalCountTask.Result);
    }

    public async Task<NoteEntity> AddAsync(NoteEntity note)
    {
        var entityEntry = await dbContext.Notes.AddAsync(note);
        await dbContext.SaveChangesAsync();
        return entityEntry.Entity;
    }

    public async Task<bool> DeleteByIdAsync(Guid noteId)
    {
        var deleted = await dbContext.Notes
            .Where(n => n.Id == noteId)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    public async Task<bool> DeleteByIdAndUserIdAsync(Guid noteId, Guid userId)
    {
        var deleted = await dbContext.Notes
            .Where(n => n.Id == noteId && n.UserId == userId)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }
}