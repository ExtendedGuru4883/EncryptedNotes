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

        var page = 1;
        const int pageSize = 1;
        var hasMore = true;
        while (hasMore)
        {
            var paginatedNotesRequest = new PaginatedNotesRequest()
            {
                Page = page,
                PageSize = pageSize
            };

            var apiGetNotesResponse =
                await ApiClient.HandleJsonGetWithAuthAsync<PaginatedResponse<NoteDto>>(
                    $"notes/get?{ToQueryString(paginatedNotesRequest)}");

            foreach (var encryptedNote in apiGetNotesResponse.Data.Items)
            {
                var plainTextTitleBytes = CryptoHelper.Decrypt(
                    Convert.FromBase64String(encryptedNote.EncryptedTitleBase64),
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