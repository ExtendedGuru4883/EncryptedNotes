using System.Text;
using Blazored.SessionStorage;
using Client.Helpers.Crypto.Interfaces;
using Client.Models;
using Client.Services.Clients.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Dto;

namespace Client.Pages;

public partial class Notes : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required ISessionStorageService SessionStorageService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly List<NoteModel> _notes = [];
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

        var apiGetNotesResponse = await ApiClient.HandleJsonGetWithAuthAsync<List<NoteDto>>("notes/getall");
        foreach (var encryptedNote in apiGetNotesResponse.Data)
        {
            var plainTextTitleBytes = CryptoHelper.Decrypt(Convert.FromBase64String(encryptedNote.EncryptedTitleBase64),
                _encryptionKeyBytes);
            var plainTextContentBytes =
                CryptoHelper.Decrypt(Convert.FromBase64String(encryptedNote.EncryptedContentBase64),
                    _encryptionKeyBytes);
            _notes.Add(new NoteModel
            {
                Title = Encoding.UTF8.GetString(plainTextTitleBytes),
                Content = Encoding.UTF8.GetString(plainTextContentBytes), 
                TimeStamp = encryptedNote.TimeStamp
            });
        }
    }
}