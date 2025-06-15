using Core.Entities;

namespace Core.Interfaces.DataAccess.Repositories;

public interface INoteRepository
{
    Task<List<NoteEntity>> GetAllByUserIdAsNoTrackingAsync(Guid userId);
}