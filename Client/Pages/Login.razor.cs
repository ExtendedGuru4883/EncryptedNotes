using System.Net.Http.Json;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;
using System.Text;
using Client.Models;
using Client.Services.Clients.Crypto.Interfaces;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace Client.Pages;

[SupportedOSPlatform("browser")]
public partial class Login : ComponentBase
{
    [Inject]
    public required HttpClient HttpClient { get; set; }
    [Inject]
    public required ICryptoService CryptoService { get; set; }
    [Inject]
    public required ISignatureService SignatureService { get; set; }
    
    private readonly LoginFormModel _model = new();
    private List<string> _errors = [];
    

    private async Task Submit()
    {
        _errors.Clear();

        var response = await HttpClient.GetAsync($"auth/GetChallenge?username={_model.Username}");
        if (!response.IsSuccessStatusCode) return;

        var challengeResponse = await response.Content.ReadFromJsonAsync<ChallengeResponse>();
        var nonceBytes = Convert.FromBase64String(challengeResponse.NonceBase64);
        var signatureSaltBytes = Convert.FromBase64String(challengeResponse.SignatureSaltBase64);
        
        var passwordBytes = Encoding.UTF8.GetBytes(_model.Password);

        var keyPairBytes = SignatureService.GenerateKeyPair(passwordBytes, signatureSaltBytes, 32);

        var signatureBytes = SignatureService.SignDetached(nonceBytes, keyPairBytes.PrivateKey);

        var loginRequest = new LoginRequest()
        {
            Username = _model.Username,
            NonceBase64 = challengeResponse.NonceBase64,
            NonceSignatureBase64 = Convert.ToBase64String(signatureBytes)
        };

        response = await HttpClient.PostAsync("auth/login", JsonContent.Create(loginRequest));
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Console.WriteLine(loginResponse.Token);
        Console.WriteLine(loginResponse.EncryptionSaltBase64);
    }
}