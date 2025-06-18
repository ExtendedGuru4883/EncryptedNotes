using System.Net.Http.Json;
using System.Text;
using Client.Exceptions;
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
    [Inject] public required ICryptoService CryptoService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required IEncryptionKeyRetrievalService EncryptionKeyRetrievalService { get; set; }
    [Inject] public required INoteService NoteService { get; set; }

    private readonly List<NoteModel> _notes = [];
    private byte[]? _encryptionKeyBytes;

    private readonly List<string> _errors = [];
    private bool _loadingNotes = true;
    private Guid _askConfirmDeletionNoteId = Guid.Empty;
    private Guid _awaitingDeletionNoteId = Guid.Empty;
    private Guid _awaitingUpdateNoteId = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        //TODO put LoadNotesAsync in INoteService and call it here removing the encryption key initialization
        try
        {
            _encryptionKeyBytes = await EncryptionKeyRetrievalService.TryGetKeyAsync();
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
            var result = await NoteService.DeleteNoteAsync(noteId);
            if (result.IsSuccess)
            {
                RemoveNoteFromLocalList(noteId);
                return;
            }

            //!apiDeleteNoteResponse.IsSuccess;
            _errors.Add(result.ErrorMessage ?? "Unexpected error deleting note");
        }
        finally
        {
            _awaitingDeletionNoteId = Guid.Empty;
            _askConfirmDeletionNoteId = Guid.Empty;
        }
    }

    private async Task SubmitEdit(NoteModel note)
    {
        _errors.Clear();
        _awaitingUpdateNoteId = note.Id;
        
        try
        {
            var result = await NoteService.UpdateNoteAsync(note.Id, note.Title, note.Content);
            if (result.IsSuccess)
            {
                note.TimeStamp = result.Data?.TimeStamp ?? DateTime.Now;
                return;
            }

            _errors.Add((string.IsNullOrEmpty(result.ErrorMessage)
                ? "Unexpected error updating note"
                : result.ErrorMessage));
        }
        catch (EncryptionKeyMissingException)
        {
            NavigationManager.NavigateTo("/login");
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
                Title = Encoding.UTF8.GetString(CryptoService.Decrypt(
                    Convert.FromBase64String(n.EncryptedTitleBase64),
                    _encryptionKeyBytes)),
                Content = Encoding.UTF8.GetString(CryptoService.Decrypt(
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

    private void RemoveNoteFromLocalList(Guid noteId)
    {
        var removed = _notes.FirstOrDefault(n => n.Id == noteId);
        if (removed != null) _notes.Remove(removed);
    }
}