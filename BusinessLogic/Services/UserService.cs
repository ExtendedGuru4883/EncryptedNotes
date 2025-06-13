using AutoMapper;
using Core.Entities;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Responses;
using Sodium;

namespace BusinessLogic.Services;

public class UserService(IUserRepository userRepository, IMapper mapper, IMemoryCache cache) : IUserService
{
    public async Task<ServiceResult<UserDto>> AddAsync(UserDto userDto)
    {
        if (await userRepository.UsernameExists(userDto.Username))
            return ServiceResult<UserDto>.Failure("Username already exists", ServiceResponseErrorType.Conflict);

        await userRepository.AddAsync(mapper.Map<UserEntity>(userDto));
        return ServiceResult<UserDto>.Success(userDto, ServiceResponseSuccessType.Created);
    }

    public async Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username)
    {
        var signatureSalt = await userRepository.GetSignatureSaltByUsername(username);
        if (signatureSalt is null)
            return ServiceResult<ChallengeResponse>.Failure("User not found", ServiceResponseErrorType.NotFound);

        var nonce = Convert.ToBase64String(Sodium.SodiumCore.GetRandomBytes(32));
        var challengeResponse = new ChallengeResponse()
        {
            SignatureSalt = signatureSalt,
            Nonce = nonce
        };

        cache.Set($"nonce:{username}", nonce, TimeSpan.FromMinutes(2));

        return ServiceResult<ChallengeResponse>.SuccessOk(challengeResponse);
    }

    public async Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        if (!cache.TryGetValue($"nonce:{loginRequest.Username}", out string? nonce) || string.IsNullOrEmpty(nonce) ||
            nonce != loginRequest.Nonce)
            return ServiceResult<LoginResponse>.Failure("Challenge expired or invalid",
                ServiceResponseErrorType.Unauthorized);

        cache.Remove($"nonce:{loginRequest.Username}");

        var publicKey = await userRepository.GetPublicKeyByUsername(loginRequest.Username);
        if (publicKey is null)
            return ServiceResult<LoginResponse>.Failure("User not found", ServiceResponseErrorType.NotFound);

        if (!PublicKeyAuth.VerifyDetached(Convert.FromBase64String(loginRequest.NonceSignature),
                Convert.FromBase64String(nonce), Convert.FromBase64String(publicKey)))
            return ServiceResult<LoginResponse>.Failure("Challenge failed: invalid signature",
                ServiceResponseErrorType.Unauthorized);

        var encryptionSalt = await userRepository.GetEncryptionSaltByUsername(loginRequest.Username);
        if (encryptionSalt is null)
            return ServiceResult<LoginResponse>.Failure("Challenge completed. User encryption salt not found",
                ServiceResponseErrorType.InternalServerError);

        var loginResponse = new LoginResponse()
        {
            Token = "pippo",
            EncryptionSalt = encryptionSalt,
        };
        
        return ServiceResult<LoginResponse>.SuccessOk(loginResponse);
    }
}