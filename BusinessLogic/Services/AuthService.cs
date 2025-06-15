using BusinessLogic.Helpers.Crypto.Interfaces;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;

namespace BusinessLogic.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    ICryptoHelper cryptoHelper,
    ISignatureHelper signatureHelper,
    IMemoryCache cache,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username)
    {
        var signatureSalt = await userRepository.GetSignatureSaltByUsername(username);
        if (string.IsNullOrEmpty(signatureSalt))
        {
            logger.LogInformation(
                "Retrieving signature salt for user {username} failed: user doesn't exist (or potential data inconsistency)",
                username);
            return ServiceResult<ChallengeResponse>.Failure("User not found", ServiceResultErrorType.NotFound);
        }

        logger.LogInformation("Retrieved signature salt for user {username}", username);

        var nonce = Convert.ToBase64String(cryptoHelper.GetRandomBytes(32));
        var challengeResponse = new ChallengeResponse()
        {
            SignatureSaltBase64 = signatureSalt,
            NonceBase64 = nonce
        };

        cache.Set($"nonceBase64:{username}", nonce, TimeSpan.FromMinutes(2));

        logger.LogInformation("Generated challenge for user {username}", username);
        return ServiceResult<ChallengeResponse>.SuccessOk(challengeResponse);
    }

    public async Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        if (!cache.TryGetValue($"nonceBase64:{loginRequest.Username}", out string? cachedNonceBase64))
        {
            logger.LogWarning("Nonce not found for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge expired or invalid",
                ServiceResultErrorType.Unauthorized);
        }

        cache.Remove($"nonceBase64:{loginRequest.Username}");
        logger.LogInformation("Nonce found for user {username} and removed from cache", loginRequest.Username);

        if (string.IsNullOrEmpty(cachedNonceBase64))
        {
            logger.LogError("Nonce is null or empty for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge corrupted",
                ServiceResultErrorType.InternalServerError);
        }

        if (cachedNonceBase64 != loginRequest.NonceBase64)
        {
            logger.LogWarning("Nonce for user {username} is valid but request nonce is different",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge invalid", ServiceResultErrorType.Unauthorized);
        }
        logger.LogInformation("Nonce for user {username} is valid and matched request nonce", loginRequest.Username);

        var userEntity = await userRepository.GetByUsernameAsNoTrackingAsync(loginRequest.Username);
        if (userEntity == null)
        {
            //This situation is only possible if the user existed in the database when the challenge was generated
            //(challenge response includes the user's signature salt retrieved form database) but doesn't exist anymore.
            //Until user deletion is implemented, this shouldn't happen under normal circumstances.
            //TODO when user deletion is implemented, decide if this should be handled differently as it would be
            //possible for a user to get a challenge, delete its account and then try to log in (won't be internal
            //server error anymore)
            logger.LogError(
                "User {username} not found in database, but valid nonce was found (so challenge generation succeeded and the user must exist). Data inconsistency?",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("User not found",
                ServiceResultErrorType.InternalServerError);
        }

        try
        {
            var signatureBytes = Convert.FromBase64String(loginRequest.NonceSignatureBase64);
            var nonceBytes = Convert.FromBase64String(cachedNonceBase64);
            var publicKeyBytes = Convert.FromBase64String(userEntity.PublicKeyBase64);
            if (!signatureHelper.VerifyDetachedSignature(signatureBytes,
                    nonceBytes, publicKeyBytes))
            {
                logger.LogInformation("Challenge failed by user {username}, invalid signature", loginRequest.Username);
                return ServiceResult<LoginResponse>.Failure("Challenge failed: invalid signature",
                    ServiceResultErrorType.Unauthorized);
            }
        }
        catch (FormatException ex)
        {
            logger.LogWarning(ex, "Invalid Base64 format in login request for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Invalid input format", ServiceResultErrorType.BadRequest);
        }

        logger.LogInformation("Challenge completed by user {username}", loginRequest.Username);

        var loginResponse = new LoginResponse()
        {
            Token = jwtService.GenerateToken(userEntity.Username, userEntity.Id),
            EncryptionSaltBase64 = userEntity.EncryptionSaltBase64,
        };
        logger.LogInformation("Login completed by user {username}. JWT generated",
            loginRequest.Username);
        return ServiceResult<LoginResponse>.SuccessOk(loginResponse);
    }
}