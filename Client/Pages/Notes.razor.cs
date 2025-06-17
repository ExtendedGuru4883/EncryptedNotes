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

public partial class Notes : ComponentBase
{
    [Inject] public required IApiClient ApiClient { get; set; }
    [Inject] public required ICryptoHelper CryptoHelper { get; set; }
    [Inject] public required ISessionStorageService SessionStorageService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly List<NoteModel> _notes = [];
    private byte[]? _encryptionKeyBytes;

    private readonly List<string> _errors = [];
    private bool _loadingNotes = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!(await TryInitializeEncryptionKey())) return;
            await LoadNotesAsync();
        }
        finally
        {
            _loadingNotes = false;
        }
    }

    private async Task<bool> TryInitializeEncryptionKey()
    {
        var encryptionKeyBase64 = await SessionStorageService.GetItemAsStringAsync("encryptionKeyBase64");
        try
        {
            _encryptionKeyBytes = Convert.FromBase64String(encryptionKeyBase64);
            return true;
        }
        catch
        {
            NavigationManager.NavigateTo("/login");
            return false;
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
            Thread.Sleep(1000);
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