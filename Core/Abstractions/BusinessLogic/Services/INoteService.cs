using Shared.Dto;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface INoteService
{
    Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser();
    Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto);
}