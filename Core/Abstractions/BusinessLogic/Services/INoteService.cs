using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface INoteService
{
    Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser();
    Task<ServiceResult<PaginatedResponse<NoteDto>>> GetPageForCurrentUser(PaginatedNotesRequest request);
    Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto);
}