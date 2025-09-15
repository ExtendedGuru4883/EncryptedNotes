using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Dto.Requests.Notes;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;

namespace BusinessLogic.Services;

public class NoteService(
    INoteRepository noteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<NoteService> logger) : INoteService
{
    [Obsolete("Deprecated. Use GetPageForCurrentUser instead.")]
    public async Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser()
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Getting notes for current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult<List<NoteDto>>.Failure("You need to be logged in to get notes",
                ServiceResultErrorType.Unauthorized);
        }

        var notes = await noteRepository.GetAllByUserIdAsync(currentUserGuid);

        logger.LogInformation("Getting notes for current user {UserId} succeeded", currentUserGuid);
        return ServiceResult<List<NoteDto>>.SuccessOk(mapper.Map<List<NoteDto>>(notes));
    }

    public async Task<ServiceResult<PageNumberPaginationResponse<NoteDto>>> GetPageByPageNumberForCurrentUser(PageNumberPaginationRequest request)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Getting paginated notes for current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult<PageNumberPaginationResponse<NoteDto>>.Failure("You need to be logged in to get notes",
                ServiceResultErrorType.Unauthorized);
        }

        var (noteEntities, totalCount) = await noteRepository.GetPageByPageNumberByUserIdAsync(currentUserGuid, request.Page, request.PageSize);
        var noteDtos = mapper.Map<List<NoteDto>>(noteEntities);
        var response = new PageNumberPaginationResponse<NoteDto>
        {
            Items = noteDtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
        

        logger.LogInformation("Getting page {Page} of size {PageSize} for current user {UserId} succeeded", request.Page, request.PageSize, currentUserGuid);
        return ServiceResult<PageNumberPaginationResponse<NoteDto>>.SuccessOk(response);
    }
    
    public async Task<ServiceResult<CursorPaginationResponse<NoteDto>>> GetPageByCursorForCurrentUser(CursorPaginationNotesRequest request)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Getting paginated notes for current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult<CursorPaginationResponse<NoteDto>>.Failure("You need to be logged in to get notes",
                ServiceResultErrorType.Unauthorized);
        }

        var (noteEntities, totalCount) = await noteRepository.GetPageByCursorByUserIdAsync(currentUserGuid, request.DateTimeCursor, request.PageSize);
        var noteDtos = mapper.Map<List<NoteDto>>(noteEntities);
        var response = new CursorPaginationResponse<NoteDto>
        {
            Items = noteDtos,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
        

        logger.LogInformation("Getting page from cursor {Cursor} of size {PageSize} for current user {UserId} succeeded", request.DateTimeCursor, request.PageSize, currentUserGuid);
        return ServiceResult<CursorPaginationResponse<NoteDto>>.SuccessOk(response);
    }

    public async Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Adding note for current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult<NoteDto>.Failure("You need to be logged in to add a note",
                ServiceResultErrorType.Unauthorized);
        }
        
        var noteEntity = mapper.Map<NoteEntity>(noteDto);
        noteEntity.UserId = currentUserGuid;
        await noteRepository.AddAsync(noteEntity);

        logger.LogInformation("Adding note for current user {UserId} succeeded", currentUserGuid);
        return ServiceResult<NoteDto>.SuccessCreated(mapper.Map<NoteDto>(noteEntity));
    }
    
    public async Task<ServiceResult> DeleteByIdForCurrentUserAsync(Guid noteId)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Deleting note {NoteId} for current user failed with unauthorized: current user id missing or invalid", noteId);
            return ServiceResult.Failure("You need to be logged in to delete a note",
                ServiceResultErrorType.Unauthorized);
        }

        if (!(await noteRepository.DeleteByIdAndUserIdAsync(noteId, currentUserGuid)))
        {
            logger.LogInformation(
                "Deleting note {NoteId} for current user {UserId} failed with not found: repository delete returned " +
                "false, note doesn't exist or doesn't belong to current user", noteId, currentUserGuid);
            return ServiceResult.Failure("Note not found",
                ServiceResultErrorType.NotFound);
        }
        
        logger.LogInformation(
            "Deleting note {NoteId} for current user {UserId} succeeded", noteId, currentUserGuid);
        return ServiceResult.SuccessNoContent();
    }
    
    public async Task<ServiceResult<NoteDto>> UpdateForCurrentUserAsync(NoteDto noteDto)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Updating note {NoteId} for current user failed with unauthorized: current user id missing or invalid", noteDto.Id);
            return ServiceResult<NoteDto>.Failure("You need to be logged in to delete a note",
                ServiceResultErrorType.Unauthorized);
        }

        if (!(await noteRepository.UpdateForUserIdAsync(mapper.Map<NoteEntity>(noteDto), currentUserGuid)))
        {
            logger.LogInformation(
                "Updating note {NoteId} for current user {UserId} failed with not found: repository delete returned " +
                "false, note doesn't exist or doesn't belong to current user", noteDto.Id, currentUserGuid);
            return ServiceResult<NoteDto>.Failure("Note not found",
                ServiceResultErrorType.NotFound);
        }
        
        logger.LogInformation(
            "Updating note {NoteId} for current user {UserId} succeeded", noteDto.Id, currentUserGuid);
        return ServiceResult<NoteDto>.SuccessOk(noteDto);
    }
}