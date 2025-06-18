using System.Net.Http.Json;
using System.Text;
using Client.Helpers;
using Client.Models;
using Client.Models.Results;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Requests.Notes;
using Shared.Dto.Responses;

namespace Client.Services;

public class NoteService(
    IEncryptionKeyRetrievalService encryptionKeyRetrievalService,
    ICryptoService cryptoService,
    IApiClient apiClient)
    : INoteService
{
    public async Task<ServiceResult<NoteModel>> AddNoteAsync(string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyRetrievalService.GetKeyOrThrowAsync();

        var encryptedTitleBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(title), encryptionKeyBytes);
        var encryptedContentBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(content), encryptionKeyBytes);

        var request = new UpsertNoteRequest
        {
            EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
            EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
        };

        var apiResponse =
            await apiClient.HandleJsonPostWithAuthAsync<NoteDto>("notes",
                JsonContent.Create(request));

        if (apiResponse is { IsSuccess: true, Data: not null })
            return ServiceResult<NoteModel>.Success(DtoToModel(apiResponse.Data, encryptionKeyBytes));

        return ServiceResult<NoteModel>.Failure(
            apiResponse.ErrorMessage ?? "Unexpected error during note creation");
    }

    public async Task<ServiceResult<NoteModel>> UpdateNoteAsync(Guid id, string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyRetrievalService.GetKeyOrThrowAsync();

        var encryptedTitleBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(title), encryptionKeyBytes);
        var encryptedContentBytes =
            cryptoService.Encrypt(Encoding.UTF8.GetBytes(content), encryptionKeyBytes);

        var request = new UpsertNoteRequest()
        {
            EncryptedTitleBase64 = Convert.ToBase64String(encryptedTitleBytes),
            EncryptedContentBase64 = Convert.ToBase64String(encryptedContentBytes),
        };

        var apiResponse =
            await apiClient.HandleJsonPutWithAuthAsync<NoteDto>($"notes/{id}",
                JsonContent.Create(request));

        if (apiResponse is { IsSuccess: true, Data: not null })
            return ServiceResult<NoteModel>.Success(DtoToModel(apiResponse.Data, encryptionKeyBytes));

        return ServiceResult<NoteModel>.Failure(
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

    public async Task<ServiceResult<(List<NoteModel> notes, bool hasMore)>> GetNotesPageAsync(int page, int pageSize)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyRetrievalService.GetKeyOrThrowAsync();

        var queryString = QueryStringHelper.ToQueryString(new PaginatedNotesRequest
        {
            Page = page,
            PageSize = pageSize
        });

        var apiResponse =
            await apiClient.HandleJsonGetWithAuthAsync<PaginatedResponse<NoteDto>>(
                $"notes?{queryString}");

        if (apiResponse is not { IsSuccess: true, Data: not null })
            return ServiceResult<(List<NoteModel>, bool)>.Failure(
                apiResponse.ErrorMessage ?? "Unexpected error during note creation");

        //apiResponse is IsSuccess true and Data not null
        var notes = apiResponse.Data.Items.Select(n => DtoToModel(n, encryptionKeyBytes)).ToList();
        return ServiceResult<(List<NoteModel> notes, bool hasMore)>.Success((notes, apiResponse.Data.HasMore));
    }

    private NoteModel DtoToModel(NoteDto dto, byte[] encryptionKeyBytes)
    {
        return new NoteModel
        {
            Id = dto.Id,
            Title = Encoding.UTF8.GetString(cryptoService.Decrypt(
                Convert.FromBase64String(dto.EncryptedTitleBase64),
                encryptionKeyBytes)),
            Content = Encoding.UTF8.GetString(cryptoService.Decrypt(
                Convert.FromBase64String(dto.EncryptedContentBase64),
                encryptionKeyBytes)),
            TimeStamp = dto.TimeStamp
        };
    }
}