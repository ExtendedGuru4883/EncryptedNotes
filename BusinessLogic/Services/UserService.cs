using AutoMapper;
using BusinessLogic.Helpers;
using Core.Entities;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Enums;
using Shared.Responses;

namespace BusinessLogic.Services;

public class UserService(
    IUserRepository userRepository,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ServiceResult<UserDto>> AddAsync(UserDto userDto)
    {
        if (await userRepository.UsernameExists(userDto.Username))
        {
            logger.LogInformation("Adding new user {username} failed: username already exists", userDto.Username);
            return ServiceResult<UserDto>.Failure("Username already exists", ServiceResponseErrorType.Conflict);
        }

        if (!Base64Helper.IsValidBase64(userDto.EncryptionSaltBase64) ||
            !Base64Helper.IsValidBase64(userDto.SignatureSaltBase64) ||
            !Base64Helper.IsValidBase64(userDto.PublicKeyBase64))
        {
            logger.LogInformation("Adding new user {username} failed: invalid base64 in request", userDto.Username);
            return ServiceResult<UserDto>.Failure(
                "EncryptionSaltBase64, SignatureSaltBase64 and PublicKeyBase64 must be valid base64 strings",
                ServiceResponseErrorType.BadRequest);
        }

        await userRepository.AddAsync(mapper.Map<UserEntity>(userDto));
        logger.LogInformation("Adding new user {username} succeeded", userDto.Username);
        return ServiceResult<UserDto>.Success(userDto, ServiceResponseSuccessType.Created);
    }
}