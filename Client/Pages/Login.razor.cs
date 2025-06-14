using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using System.Text;
using Blazored.LocalStorage;
using Client.Helpers.Crypto.Interfaces;
using Client.Models;
using Client.Services.Clients.Interfaces;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace Client.Pages;

public partial class Login : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required ISignatureHelper SignatureHelper { get; set; }
    [Inject] public required ILocalStorageService LocalStorageService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly LoginFormModel _model = new();
    private List<string> _errors = [];
    private bool _isLoading = false;


    private async Task SubmitAsync()
    {
        _errors.Clear();
        _isLoading = true;

        try
        {
            var apiChallengeResponse =
                await ApiClient.HandleJsonGetAsync<ChallengeResponse>(
                    $"auth/GetChallenge?username={_model.Username}");

            if (apiChallengeResponse is { IsSuccess: true, Data: not null })
            {
                var signatureBase64 = GetBase64DetachedChallengeSignature(apiChallengeResponse.Data);

                var loginRequest = new LoginRequest()
                {
                    Username = _model.Username,
                    NonceBase64 = apiChallengeResponse.Data.NonceBase64,
                    NonceSignatureBase64 = signatureBase64,
                };

                var apiLoginResponse =
                    await ApiClient.HandleJsonPostAsync<LoginResponse>("auth/login",
                        JsonContent.Create(loginRequest));

                if (apiLoginResponse is { IsSuccess: true, Data: not null })
                {
                    await LocalStorageService.SetItemAsStringAsync("token",
                        apiLoginResponse.Data.Token);

                    NavigationManager.NavigateTo("/");
                    return;
                }

                //!apiLoginResponse.IsSuccess
                _errors.Add(apiLoginResponse.ErrorMessage ?? "Unexpected error during login");
                return;
            }

            //!apiChallengeResponse.IsSuccess
            _errors.Add(apiChallengeResponse.ErrorMessage ?? "Unexpected error retrieving challenge");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private string GetBase64DetachedChallengeSignature(ChallengeResponse challengeResponse)
    {
        var nonceBytes = Convert.FromBase64String(challengeResponse.NonceBase64);
        var signatureSaltBytes = Convert.FromBase64String(challengeResponse.SignatureSaltBase64);

        var passwordBytes = Encoding.UTF8.GetBytes(_model.Password);

        var keyPairBytes = SignatureHelper.GenerateKeyPair(passwordBytes, signatureSaltBytes, 32);

        return Convert.ToBase64String(SignatureHelper.SignDetached(nonceBytes, keyPairBytes.PrivateKey));
    }
}