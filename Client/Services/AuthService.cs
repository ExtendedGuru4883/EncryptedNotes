using System.Net.Http.Json;
using System.Text;
using Client.Models.Results;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto.Requests.Auth;

namespace Client.Services;

public class AuthService(IApiClient apiClient, ICryptoService cryptoService, ISignatureService signatureService) : IAuthService
{
    public async Task<ServiceResult> SignupAsync(string username, string password)
    {
        var signupRequest = GenerateSignupRequest(username, password);

        var response = await apiClient.HandlePostAsync("auth/signup", JsonContent.Create(signupRequest));
        
        return response.IsSuccess ? ServiceResult.Success() : ServiceResult.Failure(response.ErrorMessage ?? "Unexpected error during signup");
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
}