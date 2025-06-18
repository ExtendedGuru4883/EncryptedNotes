using System.Net.Http.Json;
using System.Text;
using Client.Helpers.Crypto.Interfaces;
using Client.Models;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace Client.Pages;

public partial class Notes : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IEncryptionKeyService EncryptionKeyService { get; set; }

    private readonly List<NoteModel> _notes = [];
    private byte[]? _encryptionKeyBytes;

    private readonly List<string> _errors = [];
    private bool _loadingNotes = true;
    private Guid _askConfirmDeletionNoteId = Guid.Empty;
    private Guid _awaitingDeletionNoteId = Guid.Empty;
    private Guid _awaitingUpdateNoteId = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _encryptionKeyBytes = await EncryptionKeyService.TryGetKeyAsync();
            if (_encryptionKeyBytes is null)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }
            
            await LoadNotesAsync();
        }
        finally
        {
            _loadingNotes = false;
        }
    }

    private async Task DeleteNoteAsync(Guid noteId)
    {
        if (_askConfirmDeletionNoteId !=  noteId)
        {
            _askConfirmDeletionNoteId = noteId;
            StateHasChanged();
            return;
        }
        
        _awaitingDeletionNoteId = noteId;
        try
        {
            var apiDeleteNoteResponse = await ApiClient.HandleDeleteWithAuthAsync($"notes/{noteId}");
            if (apiDeleteNoteResponse.IsSuccess)
            {
                var removed = _notes.FirstOrDefault(n => n.Id == noteId);
                if (removed != null) _notes.Remove(removed);
                return;
            }

            //!apiDeleteNoteResponse.IsSuccess;
            _errors.Add(apiDeleteNoteResponse.ErrorMessage ?? "Unexpected error deleting note");
        }
        finally
        {
            _awaitingDeletionNoteId = Guid.Empty;
            _askConfirmDeletionNoteId = Guid.Empty;
        }
    }

    private async Task SubmitEdit(NoteModel note)
    {
        if (_encryptionKeyBytes is null)
        {
            NavigationManager.NavigateTo("/login");
            return;
        }
        _errors.Clear();
        _awaitingUpdateNoteId = note.Id;
        
        try
        {
            var encryptedTitleBytes =
                CryptoHelper.Encrypt(Encoding.UTF8.GetBytes(note.Title), _encryptionKeyBytes);
            var encryptedContentBytes =
                CryptoHelper.Encrypt(Encoding.UTF8.GetBytes(note.Content), _encryptionKeyBytes);
            
            var updateNoteRequest = new UpdateNoteRequest()
            {
                EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
                EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
            };

            var apiAddNoteResponse =
                await ApiClient.HandleJsonPutWithAuthAsync<NoteDto>($"notes/{note.Id}",
                    JsonContent.Create(updateNoteRequest));

            if (apiAddNoteResponse is { IsSuccess: true, Data: not null })
            {
                note.TimeStamp = apiAddNoteResponse.Data.TimeStamp;
                return;
            }

            //!apiLoginResponse.IsSuccess
            _errors.Add(apiAddNoteResponse.ErrorMessage ?? "Unexpected error during note edit");
        }
        finally
        {
            _awaitingUpdateNoteId = Guid.Empty;
        }
    }

    private async Task LoadNotesAsync()
    {
        if (_encryptionKeyBytes is null) return;

        var page = 1;
        const int pageSize = 20;
        var hasMore = true;

        while (hasMore)
        {
            var queryString = ToQueryString(new PaginatedNotesRequest
            {
                Page = page,
                PageSize = pageSize
            });

            var apiGetNotesResponse =
                await ApiClient.HandleJsonGetWithAuthAsync<PaginatedResponse<NoteDto>>(
                    $"notes?{queryString}");
            if (apiGetNotesResponse is not { IsSuccess: true, Data: not null })
            {
                //!apiGetNotesResponse.IsSuccess
                _errors.Add(apiGetNotesResponse.ErrorMessage ?? "Unexpected error loading notes");
                break;
            }

            var pageNotes = apiGetNotesResponse.Data.Items.Select(n => new NoteModel
            {
                Id = n.Id,
                Title = Encoding.UTF8.GetString(CryptoHelper.Decrypt(
                    Convert.FromBase64String(n.EncryptedTitleBase64),
                    _encryptionKeyBytes)),
                Content = Encoding.UTF8.GetString(CryptoHelper.Decrypt(
                    Convert.FromBase64String(n.EncryptedContentBase64),
                    _encryptionKeyBytes)),
                TimeStamp = n.TimeStamp
            });

            _notes.AddRange(pageNotes);
            StateHasChanged();
                
            hasMore = apiGetNotesResponse.Data.HasMore;
            page++;
        }
    }

    private static string ToQueryString(PaginatedNotesRequest paginatedNotesRequest)
    {
        var queryString = new StringBuilder();

        foreach (var property in typeof(PaginatedNotesRequest).GetProperties())
        {
            var propertyValue = property.GetValue(paginatedNotesRequest);
            if (propertyValue is null) continue;

            var propertyValueString = propertyValue.ToString();
            if (string.IsNullOrEmpty(propertyValueString)) continue;

            queryString.Append($"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(propertyValueString)}&");
        }

        if (queryString.Length > 0)
            queryString.Remove(queryString.Length - 1, 1);

        return queryString.ToString();
    }
}