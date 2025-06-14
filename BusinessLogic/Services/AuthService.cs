using BusinessLogic.Helpers.Crypto.Interfaces;
using Core.Interfaces.BusinessLogic.Services;
using Core.Interfaces.DataAccess.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Responses;

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
            return ServiceResult<ChallengeResponse>.Failure("User not found", ServiceResponseErrorType.NotFound);
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
                ServiceResponseErrorType.Unauthorized);
        }

        cache.Remove($"nonceBase64:{loginRequest.Username}");
        logger.LogInformation("Nonce found for user {username} and removed from cache", loginRequest.Username);

        if (string.IsNullOrEmpty(cachedNonceBase64))
        {
            logger.LogError("Nonce is null or empty for user {username} is valid", loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge corrupted",
                ServiceResponseErrorType.InternalServerError);
        }

        if (cachedNonceBase64 != loginRequest.NonceBase64)
        {
            logger.LogWarning("Nonce for user {username} is valid but request nonce is different",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge invalid", ServiceResponseErrorType.Unauthorized);
        }

        logger.LogInformation("Nonce for user {username} is valid and matched request nonce", loginRequest.Username);

        var publicKey = await userRepository.GetPublicKeyByUsername(loginRequest.Username);
        if (publicKey is null)
        {
            //This situation is only possible if the user exists in the database (otherwise it wouldn't have been possible
            //to generate a challenge as it needs to retrieve and send the user signature salt) but has an empty public
            //key (which is a required field in both the database and the signup request) so it indicates a problem in the database
            //TODO when user deletion is implemented, decide if this should be handled differently as it would be
            //possible for a user to get a challenge, delete its account and then try to log in
            logger.LogError(
                "Public ket not found for user {username}, but valid nonce was found (so challenge generation succeeded and the user must exist). Data inconsistency?",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Public key not found",
                ServiceResponseErrorType.InternalServerError);
        }

        try
        {
            var signatureBytes = Convert.FromBase64String(loginRequest.NonceSignatureBase64);
            var nonceBytes = Convert.FromBase64String(cachedNonceBase64);
            var publicKeyBytes = Convert.FromBase64String(publicKey);
            if (!signatureHelper.VerifyDetachedSignature(signatureBytes,
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
            //This situation is only possible if the user exists in the database (otherwise it wouldn't have been possible
            //to generate a challenge as it needs to retrieve and send the user signature salt) but has an empty encryption
            //salt (which is a required field in both the database and the signup request) so it indicates a problem in the database
            //TODO when user deletion is implemented, decide if this should be handled differently as it would be
            //possible for a user to get a challenge, delete its account and then try to log in
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