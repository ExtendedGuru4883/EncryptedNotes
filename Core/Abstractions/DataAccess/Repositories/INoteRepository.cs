using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface INoteRepository
{
    Task<List<NoteEntity>> GetAllByUserIdAsync(Guid userId);
    Task<(List<NoteEntity> notes, int totalCount)> GetPageByUserIdAsync(Guid userId, int page, int pageSize);
    Task<NoteEntity> AddAsync(NoteEntity note);
}