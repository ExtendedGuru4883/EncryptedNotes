using AutoMapper;
using BusinessLogic.Helpers.Crypto.Interfaces;
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
    ISignatureHelper signatureHelper,
    IMapper mapper,
    ILogger<UserService> logger) : IUserService
{
    public async Task<ServiceResult<UserDto>> AddAsync(UserDto userDto)
    {
        if ((Convert.FromBase64String(userDto.PublicKeyBase64)).Length != signatureHelper.PublicKeyBytesSize)
        {
            logger.LogInformation("Adding new user {username} failed with bad request: invalid public key size", userDto.Username);
            return ServiceResult<UserDto>.Failure("Invalid public key size", ServiceResultErrorType.BadRequest);
        }
        
        if (await userRepository.UsernameExists(userDto.Username))
        {
            logger.LogInformation("Adding new user {username} failed  with conflict: username already exists", userDto.Username);
            return ServiceResult<UserDto>.Failure("Username already exists", ServiceResultErrorType.Conflict);
        }

        await userRepository.AddAsync(mapper.Map<UserEntity>(userDto));
        logger.LogInformation("Adding new user {username} succeeded", userDto.Username);
        return ServiceResult<UserDto>.Success(userDto, ServiceResultSuccessType.Created);
    }
}