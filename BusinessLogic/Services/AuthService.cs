using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Responses;
using Sodium;

namespace BusinessLogic.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    IMemoryCache cache,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username)
    {
        var signatureSalt = await userRepository.GetSignatureSaltByUsername(username);
        if (signatureSalt is null)
        {
            logger.LogInformation("Retrieving signature salt for user {username} failed: user doesn't exist", username);
            return ServiceResult<ChallengeResponse>.Failure("User not found", ServiceResponseErrorType.NotFound);
        }

        logger.LogInformation("Retrieved signature salt for user {username}", username);

        var nonce = Convert.ToBase64String(SodiumCore.GetRandomBytes(32));
        var challengeResponse = new ChallengeResponse()
        {
            SignatureSaltBase64 = signatureSalt,
            NonceBase64 = nonce
        };

        cache.Set($"nonce:{username}", nonce, TimeSpan.FromMinutes(2));

        logger.LogInformation("Generated challenge for user {username}", username);
        return ServiceResult<ChallengeResponse>.SuccessOk(challengeResponse);
    }

    public async Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        if (!cache.TryGetValue($"nonce:{loginRequest.Username}", out string? nonce))
        {
            logger.LogWarning("Nonce not found for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge expired or invalid",
                ServiceResponseErrorType.Unauthorized);
        }

        if (string.IsNullOrEmpty(nonce))
        {
            logger.LogError("Nonce found but null or empty for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge corrupted",
                ServiceResponseErrorType.InternalServerError);
        }

        if (nonce != loginRequest.NonceBase64)
        {
            logger.LogWarning("Nonce found for user {username} but request nonce is different", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge invalid", ServiceResponseErrorType.Unauthorized);
        }

        cache.Remove($"nonce:{loginRequest.Username}");
        logger.LogInformation("Valid nonce found for user {username}. Nonce removed from cache", loginRequest.Username);

        var publicKey = await userRepository.GetPublicKeyByUsername(loginRequest.Username);
        if (publicKey is null)
        {
            logger.LogError(
                "Public ket not found for user {username}, but valid nonce was found. Potential data inconsistency.",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("User not found", ServiceResponseErrorType.NotFound);
        }

        try
        {
            var signatureBytes = Convert.FromBase64String(loginRequest.NonceSignatureBase64);
            var nonceBytes = Convert.FromBase64String(loginRequest.NonceBase64);
            var publicKeyBytes = Convert.FromBase64String(publicKey);
            if (!PublicKeyAuth.VerifyDetached(signatureBytes,
                    nonceBytes, publicKeyBytes))
            {
                logger.LogInformation("Challenge failed by user {username}, invalid signature", loginRequest.Username);
                return ServiceResult<LoginResponse>.Failure("Challenge failed: invalid signature",
                    ServiceResponseErrorType.Unauthorized);
            }
        }
        catch (FormatException ex)
        {
            logger.LogWarning(ex, "Invalid Base64 format in login request for user {username}", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Invalid input format", ServiceResponseErrorType.BadRequest);
        }

        logger.LogInformation("Challenge completed by user {username}", loginRequest.Username);

        var encryptionSalt = await userRepository.GetEncryptionSaltByUsername(loginRequest.Username);
        if (encryptionSalt is null)
        {
            logger.LogError(
                "Challenge completed by user {username}, but encryption salt not found. Potential data inconsistency.",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge completed. User encryption salt not found",
                ServiceResponseErrorType.InternalServerError);
        }

        var loginResponse = new LoginResponse()
        {
            Token = jwtService.GenerateToken(loginRequest.Username),
            EncryptionSaltBase64 = encryptionSalt,
        };
        logger.LogInformation("Login completed by user {username}. Generated JWT and retrieved encryption salt",
            loginRequest.Username);
        return ServiceResult<LoginResponse>.SuccessOk(loginResponse);
    }
}