using System.Net.Http.Json;
using System.Text;
using Client.Exceptions;
using Client.Models.Results;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto;
using Shared.Dto.Requests;

namespace Client.Services;

public class NoteService(IEncryptionKeyRetrievalService encryptionKeyRetrievalService, ICryptoService cryptoService, IApiClient apiClient)
    : INoteService
{
    public async Task<ServiceResult<NoteDto>> AddNoteAsync(string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyRetrievalService.GetKeyOrThrowAsync();

        var encryptedTitleBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(title), encryptionKeyBytes);
        var encryptedContentBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(content), encryptionKeyBytes);
        
        var request = new AddNoteRequest
        {
            EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
            EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
        };

        var apiResponse =
            await apiClient.HandleJsonPostWithAuthAsync<NoteDto>("notes",
                JsonContent.Create(request));

        if (apiResponse is { IsSuccess: true, Data: not null })
            return ServiceResult<NoteDto>.Success(apiResponse.Data);

        return ServiceResult<NoteDto>.Failure(
            apiResponse.ErrorMessage ?? "Unexpected error during note creation");
    }

    public async Task<ServiceResult<NoteDto>> UpdateNoteAsync(Guid id, string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyRetrievalService.GetKeyOrThrowAsync();

        var encryptedTitleBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(title), encryptionKeyBytes);
        var encryptedContentBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(content), encryptionKeyBytes);
            
        var request = new UpdateNoteRequest()
        {
            EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
            EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
        };

        var apiResponse =
            await apiClient.HandleJsonPutWithAuthAsync<NoteDto>($"notes/{id}",
                JsonContent.Create(request));

        if (apiResponse is { IsSuccess: true, Data: not null })
            return ServiceResult<NoteDto>.Success(apiResponse.Data);

        return ServiceResult<NoteDto>.Failure(
            apiResponse.ErrorMessage ?? "Unexpected error updating note");
    }

    public async Task<ServiceResult> DeleteNoteAsync(Guid id)
    {
        var apiResponse = await apiClient.HandleDeleteWithAuthAsync($"notes/{id}");
        
        if (apiResponse.IsSuccess)
            return ServiceResult.Success();

        return ServiceResult.Failure(
            apiResponse.ErrorMessage ?? "Unexpected error deleting note");
    }
}