using System.Net.Http.Json;
using System.Text;
using Blazored.SessionStorage;
using Client.Helpers.Crypto.Interfaces;
using Client.Models;
using Client.Services.Clients.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace Client.Pages;

public partial class AddNote : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required ISessionStorageService SessionStorageService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly AddNoteFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading = true;
    private byte[]? _encryptionKeyBytes;

    protected override async Task OnInitializedAsync()
    {
        var encryptionKeyBase64 = await SessionStorageService.GetItemAsStringAsync("encryptionKeyBase64");
        try
        {
            _encryptionKeyBytes = Convert.FromBase64String(encryptionKeyBase64);
        }
        catch
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
                await ApiClient.HandleJsonPostWithAuthAsync<NoteDto>("notes/add",
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