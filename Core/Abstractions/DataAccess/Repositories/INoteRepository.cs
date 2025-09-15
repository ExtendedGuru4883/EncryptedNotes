using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface INoteRepository
{
    Task<List<NoteEntity>> GetAllByUserIdAsync(Guid userId);
    Task<(List<NoteEntity> notes, int totalCount)> GetPageByPageNumberByUserIdAsync(Guid userId, int page, int pageSize);
    Task<(List<NoteEntity> notes, int totalCount)> GetPageByCursorByUserIdAsync(Guid userId, DateTime cursor, int pageSize);
    Task<NoteEntity> AddAsync(NoteEntity note);
    Task<bool> DeleteByIdAsync(Guid noteId);
    Task<bool> DeleteByIdAndUserIdAsync(Guid noteId, Guid userId);
    Task<bool> UpdateForUserIdAsync(NoteEntity note, Guid userId);
}