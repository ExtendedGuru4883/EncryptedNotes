using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Dto.Requests;
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
            return ServiceResult<List<NoteDto>>.Failure("There is no authenticated user",
                ServiceResultErrorType.Unauthorized);
        }

        var notes = await noteRepository.GetAllByUserIdAsync(currentUserGuid);

        logger.LogInformation("Getting notes for current user {UserId} succeeded", currentUserGuid);
        return ServiceResult<List<NoteDto>>.SuccessOk(mapper.Map<List<NoteDto>>(notes));
    }

    public async Task<ServiceResult<PaginatedResponse<NoteDto>>> GetPageForCurrentUser(PaginatedNotesRequest request)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Getting paginated notes for current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult<PaginatedResponse<NoteDto>>.Failure("There is no authenticated user",
                ServiceResultErrorType.Unauthorized);
        }

        var (noteEntities, totalCount) = await noteRepository.GetPageByUserIdAsync(currentUserGuid, request.Page, request.PageSize);
        var noteDtos = mapper.Map<List<NoteDto>>(noteEntities);
        var response = new PaginatedResponse<NoteDto>
        {
            Items = noteDtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
        

        logger.LogInformation("Getting page {Page} of size {PageSize} for current user {UserId} succeeded:", request.Page, request.PageSize, currentUserGuid);
        return ServiceResult<PaginatedResponse<NoteDto>>.SuccessOk(response);
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
        return ServiceResult<NoteDto>.Success(mapper.Map<NoteDto>(noteEntity), ServiceResultSuccessType.Created);
    }
}