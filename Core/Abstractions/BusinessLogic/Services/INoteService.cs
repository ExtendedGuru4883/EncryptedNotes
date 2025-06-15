using Shared.Dto;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface INoteService
{
    public Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser();
}