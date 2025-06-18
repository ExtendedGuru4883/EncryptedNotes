using Client.Models.Results;
using Shared.Dto;

namespace Client.Services.Interfaces;

public interface INoteService
{
    Task<ServiceResult<NoteDto>> AddNoteAsync(string title, string content);
    Task<ServiceResult<NoteDto>> UpdateNoteAsync(Guid id, string title, string content);
    Task<ServiceResult> DeleteNoteAsync(Guid id);
}