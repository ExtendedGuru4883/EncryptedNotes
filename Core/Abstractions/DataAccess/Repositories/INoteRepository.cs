using Core.Entities;

namespace Core.Abstractions.DataAccess.Repositories;

public interface INoteRepository
{
    Task<List<NoteEntity>> GetAllByUserIdAsNoTrackingAsync(Guid userId);
}