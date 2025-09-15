using System.Net.Http.Json;
using System.Text;
using Client.Helpers;
using Client.Models;
using Client.Models.Results;
using Client.Services.Clients.Interfaces;
using Client.Services.Interfaces;
using Shared.Dto;
using Shared.Dto.Requests.Notes;
using Shared.Dto.Responses;

namespace Client.Services;

public class NoteService(
    IEncryptionKeyStorageService encryptionKeyStorageService,
    ICryptoService cryptoService,
    IApiClient apiClient)
    : INoteService
{
    public async Task<ServiceResult<NoteModel>> AddNoteAsync(string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyStorageService.GetKeyOrThrowAsync();

        var request = CreateUpsertNoteRequest(title, content, encryptionKeyBytes);

        var apiResponse =
            await apiClient.HandleJsonPostWithAuthAsync<NoteDto>("notes",
                JsonContent.Create(request));

        return apiResponse is { IsSuccess: true, Data: not null }
            ? ServiceResult<NoteModel>.Success(DtoToModel(apiResponse.Data, encryptionKeyBytes))
            : ServiceResult<NoteModel>.Failure(
                apiResponse.ErrorMessage ?? "Unexpected error during note creation");
    }

    public async Task<ServiceResult<NoteModel>> UpdateNoteAsync(Guid id, string title, string content)
    {
        //This throws EncryptionKeyMissingException
        var encryptionKeyBytes = await encryptionKeyStorageService.GetKeyOrThrowAsync();

        var request = CreateUpsertNoteRequest(title, content, encryptionKeyBytes);

        var apiResponse =
            await apiClient.HandleJsonPutWithAuthAsync<NoteDto>($"notes/{id}",
                JsonContent.Create(request));

        return apiResponse is { IsSuccess: true, Data: not null }
            ? ServiceResult<NoteModel>.Success(DtoToModel(apiResponse.Data, encryptionKeyBytes))
            : ServiceResult<NoteModel>.Failure(
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
        var encryptionKeyBytes = await encryptionKeyStorageService.GetKeyOrThrowAsync();

        var queryString = QueryStringHelper.ToQueryString(new PageNumberPaginationRequest
        {
            Page = page,
            PageSize = pageSize
        });

        var apiResponse =
            await apiClient.HandleJsonGetWithAuthAsync<PageNumberPaginationResponse<NoteDto>>(
                $"notes/byPageNumber?{queryString}");

        return apiResponse is { IsSuccess: true, Data: not null }
            ? ServiceResult<(List<NoteModel> notes, bool hasMore)>.Success((
                apiResponse.Data.Items.Select(n => DtoToModel(n, encryptionKeyBytes)).ToList(),
                apiResponse.Data.HasMore))
            : ServiceResult<(List<NoteModel>, bool)>.Failure(
                apiResponse.ErrorMessage ?? "Unexpected error during note creation");
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

    private UpsertNoteRequest CreateUpsertNoteRequest(string title, string content, byte[] encryptionKeyBytes)
    {
        return new UpsertNoteRequest
        {
            EncryptedTitleBase64 =
                Convert.ToBase64String(cryptoService.Encrypt(Encoding.UTF8.GetBytes(title), encryptionKeyBytes)),
            EncryptedContentBase64 =
                Convert.ToBase64String(cryptoService.Encrypt(Encoding.UTF8.GetBytes(content), encryptionKeyBytes)),
        };
    }
}