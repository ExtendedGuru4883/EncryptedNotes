using System.Net.Http.Json;
using System.Text;
using Client.Helpers.Crypto.Interfaces;
using Client.Models;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Dto;
using Shared.Dto.Requests;

namespace Client.Pages;

public partial class AddNote : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IEncryptionKeyService EncryptionKeyService { get; set; }

    private readonly AddNoteFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading = true;
    private byte[]? _encryptionKeyBytes;

    protected override async Task OnInitializedAsync()
    {
        _encryptionKeyBytes = await EncryptionKeyService.TryGetKeyAsync();
        if (_encryptionKeyBytes is null)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }
        _isLoading = false;
    }

    private async Task SubmitAsync()
    {
        if (_encryptionKeyBytes is null)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }

        _errors.Clear();
        _isLoading = true;

        try
        {
            var encryptedTitleBytes =
                CryptoHelper.Encrypt(Encoding.UTF8.GetBytes(_model.Title), _encryptionKeyBytes);
            var encryptedContentBytes =
                CryptoHelper.Encrypt(Encoding.UTF8.GetBytes(_model.Content), _encryptionKeyBytes);
            var addNoteRequest = new AddNoteRequest
            {
                EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
                EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
            };

            var apiAddNoteResponse =
                await ApiClient.HandleJsonPostWithAuthAsync<NoteDto>("notes",
                    JsonContent.Create(addNoteRequest));

            if (apiAddNoteResponse is { IsSuccess: true, Data: not null })
            {
                _model.Title = string.Empty;
                _model.Content = string.Empty;
                return;
            }

            //!apiLoginResponse.IsSuccess
            _errors.Add(apiAddNoteResponse.ErrorMessage ?? "Unexpected error during note creation");
        }
        finally
        {
            _isLoading = false;
        }
    }
}