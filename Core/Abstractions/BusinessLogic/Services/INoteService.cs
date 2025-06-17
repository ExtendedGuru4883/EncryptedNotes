using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface INoteService
{
    [Obsolete("Deprecated. Use GetPageForCurrentUser instead.")]
    Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser();
    Task<ServiceResult<PaginatedResponse<NoteDto>>> GetPageForCurrentUser(PaginatedNotesRequest request);
    Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto);
    Task<ServiceResult> DeleteByIdForCurrentUserAsync(Guid noteId);
}