using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Enums;
using Shared.Results;

namespace BusinessLogic.Services;

public class NoteService(
    INoteRepository noteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<NoteService> logger) : INoteService
{
    public async Task<ServiceResult<List<NoteDto>>> GetAllForCurrentUser()
    {
        var currentUserId = currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            logger.LogInformation(
                "Getting notes for current user failed with unauthorized: current user id is null or empty");
            return ServiceResult<List<NoteDto>>.Failure("There is no authenticated user",
                ServiceResultErrorType.Unauthorized);
        }

        var notes = await noteRepository.GetAllByUserIdAsNoTrackingAsync(Guid.Parse(currentUserId));

        if (notes.Count == 0)
        {
            logger.LogInformation(
                "Getting notes for current user {currentUserId} failed with not found: current user has no notes",
                currentUserId);
            return ServiceResult<List<NoteDto>>.Failure("No notes found for the authenticated user",
                ServiceResultErrorType.NotFound);
        }

        logger.LogInformation("Getting notes for current user {currentUserId} succeeded", currentUserId);
        return ServiceResult<List<NoteDto>>.SuccessOk(mapper.Map<List<NoteDto>>(notes));
    }

    public async Task<ServiceResult<NoteDto>> AddAsyncToCurrentUser(NoteDto noteDto)
    {
        var currentUserId = currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            logger.LogInformation(
                "Adding note for current user failed with unauthorized: current user id is null or empty");
            return ServiceResult<NoteDto>.Failure("You need to be logged in to add a note",
                ServiceResultErrorType.Unauthorized);
        }

        try
        {
            _ = Convert.FromBase64String(noteDto.EncryptedTitleBase64);
            _ = Convert.FromBase64String(noteDto.EncryptedContentBase64);
        }
        catch (FormatException)
        {
            logger.LogInformation("Adding note for current user {currentUserId} failed with bad request: invalid base64 in request", currentUserId);
            return ServiceResult<NoteDto>.Failure(
                "EncryptedTitleBase64 and EncryptedContentBase64 must be valid base64 strings",
                ServiceResultErrorType.BadRequest);
        }


        var noteEntity = mapper.Map<NoteEntity>(noteDto);
        noteEntity.UserId = Guid.Parse(currentUserId);
        await noteRepository.AddAsync(noteEntity);
        
        logger.LogInformation("Adding note for current user {currentUserId} succeeded", currentUserId);
        return ServiceResult<NoteDto>.Success(noteDto, ServiceResultSuccessType.Created);
    }
}