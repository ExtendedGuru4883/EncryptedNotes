using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Enums;
using Shared.Results;

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
            return ServiceResult<UserDto>.Failure("Username already exists", ServiceResultErrorType.Conflict);
        }

        try
        {
            _ = Convert.FromBase64String(userDto.EncryptionSaltBase64);
            _ = Convert.FromBase64String(userDto.SignatureSaltBase64);
            _ = Convert.FromBase64String(userDto.PublicKeyBase64);
        }
        catch (FormatException)
        {
            logger.LogInformation("Adding new user {username} failed: invalid base64 in request", userDto.Username);
            return ServiceResult<UserDto>.Failure(
                "EncryptionSaltBase64, SignatureSaltBase64 and PublicKeyBase64 must be valid base64 strings",
                ServiceResultErrorType.BadRequest);
        }

        await userRepository.AddAsync(mapper.Map<UserEntity>(userDto));
        logger.LogInformation("Adding new user {username} succeeded", userDto.Username);
        return ServiceResult<UserDto>.Success(userDto, ServiceResultSuccessType.Created);
    }
}