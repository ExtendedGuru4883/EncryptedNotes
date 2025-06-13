using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using System.Text;
using Client.Models;
using Client.Services.Crypto.Interfaces;
using Shared.Dto.Requests;

namespace Client.Pages;

public partial class Signup : ComponentBase
{
    [Inject]
    public required HttpClient HttpClient { get; set; } 
    [Inject]
    public required ICryptoService CryptoService { get; set; }
    [Inject]
    public required ISignatureService SignatureService { get; set; }
    
    private readonly SignupFormModel _model = new();
    private List<string> _errors = [];

    private async Task Submit()
    {
        _errors.Clear();

        var signatureSaltBytes = CryptoService.GenerateSalt(16);
        var passwordBytes = Encoding.UTF8.GetBytes(_model.Password);

        var keyPairBytes = SignatureService.GenerateKeyPair(passwordBytes, signatureSaltBytes, 32);

        var signupRequest = new SignupRequest()
        {
            Username = _model.Username,
            SignatureSaltBase64 = Convert.ToBase64String(signatureSaltBytes),
            EncryptionSaltBase64 = Convert.ToBase64String(CryptoService.GenerateSalt(16)),
            PublicKeyBase64 = Convert.ToBase64String(keyPairBytes.PublicKey)
        };

        var response = await HttpClient.PostAsync("auth/signup", JsonContent.Create(signupRequest));
    }
}