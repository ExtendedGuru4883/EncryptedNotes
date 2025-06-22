using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Enums;
using Shared.Helpers;
using Shared.Results;

namespace BusinessLogic.Services;

public class UserService(
    IUserRepository userRepository,
    ISignatureService signatureService,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ServiceResult<UserDto>> AddAsync(UserDto userDto)
    {
        if (userDto.PublicKeyBase64.Length != signatureService.PublicKeyBase64Length)
        {
            logger.LogInformation("Adding new user {username} failed with bad request: invalid public key size",
                SanitizeForLogging.Sanitize(userDto.Username));
            return ServiceResult<UserDto>.Failure(
                $"Invalid public key size. Base64 key size must be {signatureService.PublicKeyBase64Length} characters",
                ServiceResultErrorType.BadRequest);
        }

        if (await userRepository.UsernameExistsAsync(userDto.Username))
        {
            logger.LogInformation("Adding new user {username} failed  with conflict: username already exists",
                SanitizeForLogging.Sanitize(userDto.Username));
            return ServiceResult<UserDto>.Failure("Username already exists", ServiceResultErrorType.Conflict);
        }

        await userRepository.AddAsync(mapper.Map<UserEntity>(userDto));
        logger.LogInformation("Adding new user {username} succeeded", SanitizeForLogging.Sanitize(userDto.Username));
        return ServiceResult<UserDto>.SuccessCreated(userDto);
    }

    public async Task<ServiceResult> DeleteCurrentAsync()
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserGuid))
        {
            logger.LogInformation(
                "Deleting current user failed with unauthorized: current user id missing or invalid");
            return ServiceResult.Failure("You need to be logged in to delete your account",
                ServiceResultErrorType.Unauthorized);
        }

        if (!(await userRepository.DeleteByIdAsync(currentUserGuid)))
        {
            logger.LogInformation(
                "Deleting current user {UserId} failed with not found: repository delete returned false, user probably" +
                " doesn't exist", currentUserGuid);
            return ServiceResult.Failure("User not found",
                ServiceResultErrorType.NotFound);
        }

        logger.LogInformation(
            "Deleting current user {UserId} succeeded", currentUserGuid);
        return ServiceResult.SuccessNoContent();
    }
}