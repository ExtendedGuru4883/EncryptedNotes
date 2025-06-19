using Core.Abstractions.BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shared.Dto.Requests.Auth;
using Shared.Dto.Responses;
using Shared.Enums;
using Shared.Results;

namespace BusinessLogic.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService,
    ICryptoService cryptoService,
    ISignatureService signatureService,
    IMemoryCache cache,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username)
    {
        var signatureSalt = await userRepository.GetSignatureSaltByUsernameAsync(username);
        if (string.IsNullOrEmpty(signatureSalt))
        {
            logger.LogInformation(
                "Retrieving signature salt for user {username} failed with not found: user doesn't exist (or potential data inconsistency)",
                username);
            return ServiceResult<ChallengeResponse>.Failure("User not found", ServiceResultErrorType.NotFound);
        }

        logger.LogInformation("Retrieved signature salt for user {username}", username);

        var nonce = Convert.ToBase64String(cryptoService.GetRandomBytes(32));
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
        if (loginRequest.NonceSignatureBase64.Length != signatureService.SignatureBase64Length)
        {
            logger.LogInformation(
                "Login for user {username} failed with bad request: invalid signature size",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge failed: invalid signature size",
                ServiceResultErrorType.BadRequest);
        }
        
        var signatureBytes = Convert.FromBase64String(loginRequest.NonceSignatureBase64);

        if (!cache.TryGetValue($"nonceBase64:{loginRequest.Username}", out string? cachedNonceBase64) ||
            string.IsNullOrEmpty(cachedNonceBase64) || cachedNonceBase64 != loginRequest.NonceBase64)
        {
            logger.LogInformation("Login for user {username} failed with unauthorized",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge expired or invalid",
                ServiceResultErrorType.Unauthorized);
        }

        logger.LogInformation("Nonce for user {username} is valid and matched request nonce", loginRequest.Username);

        var userEntity = await userRepository.GetByUsernameAsync(loginRequest.Username);
        if (userEntity == null)
        {
            //This situation is only possible if the user generated a challenge, deleted its account and sent a login
            //Request with the old still valid authentication token
            logger.LogInformation(
                "Login for user {username} failed with not found: user not found in database, " +
                "probably generated challenge, deleted account and sent login request with old token?)",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("User not found",
                ServiceResultErrorType.NotFound);
        }

        logger.LogInformation("Successfully retrieved user {username} from database", loginRequest.Username);

        var nonceBytes = Convert.FromBase64String(cachedNonceBase64);
        var publicKeyBytes = Convert.FromBase64String(userEntity.PublicKeyBase64);

        if (!signatureService.VerifyDetachedSignature(signatureBytes,
                nonceBytes, publicKeyBytes))
        {
            logger.LogInformation(
                "Login for user {username} failed with unauthorized: challenge failed, invalid signature",
                loginRequest.Username);
            return ServiceResult<LoginResponse>.Failure("Challenge failed: invalid signature",
                ServiceResultErrorType.Unauthorized);
        }

        logger.LogInformation("Challenge completed by user {username}", loginRequest.Username);

        var loginResponse = new LoginResponse()
        {
            Token = jwtService.GenerateToken(userEntity.Username, userEntity.Id),
            EncryptionSaltBase64 = userEntity.EncryptionSaltBase64,
        };
        logger.LogInformation("Login for user {username} succeeded. JWT generated",
            loginRequest.Username);
        return ServiceResult<LoginResponse>.SuccessOk(loginResponse);
    }
}