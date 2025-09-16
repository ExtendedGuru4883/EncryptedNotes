using System.Net.Http.Json;
using System.Text;
using Blazored.SessionStorage;
using Client.Models.Results;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto.Requests.Auth;
using Shared.Dto.Responses;
using Shared.Dto.Responses.Auth;

namespace Client.Services;

public class AuthService(
    IApiClient apiClient,
    ICryptoService cryptoService,
    ISignatureService signatureService,
    IAuthStateService authStateService,
    ISessionStorageService sessionStorageService) : IAuthService
{
    public async Task<ServiceResult> SignupAsync(string username, string password)
    {
        var request = GenerateSignupRequest(username, password);

        var response = await apiClient.HandlePostAsync("auth/signup", JsonContent.Create(request));

        return response.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(response.ErrorMessage ?? "Unexpected error during signup");
    }

    public async Task<ServiceResult> LoginAsync(string username, string password)
    {
        var apiChallengeResponse =
            await apiClient.HandleJsonGetAsync<ChallengeResponse>(
                $"auth/Challenge?username={username}");

        if (apiChallengeResponse is not { IsSuccess: true, Data: not null })
            return ServiceResult.Failure(apiChallengeResponse.ErrorMessage ?? "Unexpected error getting challenge");

        //apiChallengeResponse is IsSuccess true, Data not null
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var loginRequest = GenerateLoginRequest(apiChallengeResponse.Data, passwordBytes, username);

        var apiLoginResponse =
            await apiClient.HandleJsonPostAsync<LoginResponse>("auth/login",
                JsonContent.Create(loginRequest));

        if (apiLoginResponse is not { IsSuccess: true, Data: not null })
            return ServiceResult.Failure(apiLoginResponse.ErrorMessage ?? "Unexpected error during login");

        //apiLoginResponse is IsSuccess true, Data not null
        await sessionStorageService.SetItemAsStringAsync("token",
            apiLoginResponse.Data.Token);

        var encryptionKeyBytes = cryptoService.DeriveEncryptionKey(
            passwordBytes,
            Convert.FromBase64String(apiLoginResponse.Data.EncryptionSaltBase64));
        var encryptionKeyBase64 = Convert.ToBase64String(encryptionKeyBytes);

        await sessionStorageService.SetItemAsStringAsync("encryptionKeyBase64", encryptionKeyBase64);

        authStateService.IsLoggedIn = true;
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> LogoutAsync()
    {
        await sessionStorageService.ClearAsync();
        authStateService.IsLoggedIn = false;
        return ServiceResult.Success();
    }

    private SignupRequest GenerateSignupRequest(string username, string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var signatureSaltBytes = cryptoService.GenerateSalt();
        var encryptionSaltBytes = cryptoService.GenerateSalt();
        var keyPairBytes = signatureService.GenerateKeyPair(passwordBytes, signatureSaltBytes);

        return new SignupRequest()
        {
            Username = username,
            SignatureSaltBase64 = Convert.ToBase64String(signatureSaltBytes),
            EncryptionSaltBase64 = Convert.ToBase64String(encryptionSaltBytes),
            PublicKeyBase64 = Convert.ToBase64String(keyPairBytes.PublicKey)
        };
    }

    private LoginRequest GenerateLoginRequest(ChallengeResponse challengeResponse, byte[] passwordBytes,
        string username)
    {
        var nonceSignatureBytes = SignChallengeDetached(challengeResponse, passwordBytes);

        return new LoginRequest()
        {
            Username = username,
            NonceBase64 = challengeResponse.NonceBase64,
            NonceSignatureBase64 = Convert.ToBase64String(nonceSignatureBytes),
        };
    }

    private byte[] SignChallengeDetached(ChallengeResponse challengeResponse, byte[] passwordBytes)
    {
        var nonceBytes = Convert.FromBase64String(challengeResponse.NonceBase64);
        var signatureSaltBytes = Convert.FromBase64String(challengeResponse.SignatureSaltBase64);

        var keyPairBytes = signatureService.GenerateKeyPair(passwordBytes, signatureSaltBytes);

        return signatureService.SignDetached(nonceBytes, keyPairBytes.PrivateKey);
    }
}