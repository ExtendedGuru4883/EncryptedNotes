using Client.Models;
using Client.Models.Results;

namespace Client.Services.Interfaces;

public interface INoteService
{
    Task<ServiceResult<NoteModel>> AddNoteAsync(string title, string content);
    Task<ServiceResult<NoteModel>> UpdateNoteAsync(Guid id, string title, string content);
    Task<ServiceResult> DeleteNoteAsync(Guid id);
    Task<ServiceResult<(List<NoteModel> notes, bool hasMore)>> GetNotesPageAsync(int page, int pageSize);
}