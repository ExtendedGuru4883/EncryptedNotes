using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using System.Text;
using Client.Models.Forms;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto.Requests;
using Shared.Dto.Requests.Auth;

namespace Client.Pages;

public partial class Signup : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoService CryptoService { get; set; }
    [Inject] public required ISignatureService SignatureService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly SignupFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading;

    private async Task Submit()
    {
        _errors.Clear();
        _isLoading = true;

        try
        {
            var signupRequest = GenerateSignupRequest();

            var response = await ApiClient.HandlePostAsync("auth/signup", JsonContent.Create(signupRequest));
            if (response.IsSuccess)
            {
                NavigationManager.NavigateTo("login");
                return;
            }
            _errors.Add(response.ErrorMessage ?? "Unexpected error during signup");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private SignupRequest GenerateSignupRequest()
    {
        var passwordBytes = Encoding.UTF8.GetBytes(_model.Password);
        var signatureSaltBytes = CryptoService.GenerateSalt();

        var keyPairBytes = SignatureService.GenerateKeyPair(passwordBytes, signatureSaltBytes);

        return new SignupRequest()
        {
            Username = _model.Username,
            SignatureSaltBase64 = Convert.ToBase64String(signatureSaltBytes),
            EncryptionSaltBase64 = Convert.ToBase64String(CryptoService.GenerateSalt()),
            PublicKeyBase64 = Convert.ToBase64String(keyPairBytes.PublicKey)
        };
    }
}