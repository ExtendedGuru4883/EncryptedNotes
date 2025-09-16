using Shared.Dto;
using Shared.Dto.Requests.Notes;
using Shared.Dto.Responses;
using Shared.Dto.Responses.Notes;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface INoteService
{
    [Obsolete("Deprecated. Use GetPageForCurrentUser instead.")]
    Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser();
    Task<ServiceResult<PageNumberPaginationResponse<NoteDto>>> GetPageByPageNumberForCurrentUser(PageNumberPaginationRequest request);
    Task<ServiceResult<CursorPaginationResponse<NoteDto>>> GetPageByCursorForCurrentUser(CursorPaginationNotesRequest request);
    Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto);
    Task<ServiceResult> DeleteByIdForCurrentUserAsync(Guid noteId);
    Task<ServiceResult<NoteDto>> UpdateForCurrentUserAsync(NoteDto noteDto);
}