using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.SessionStorage;
using Client.Models;
using Client.Models.Responses;
using Client.Services.Clients.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Dto.Responses;

namespace Client.Services.Clients;

public class ApiClient(
    HttpClient httpClient,
    ISessionStorageService sessionStorageService,
    NavigationManager navigationManager) : IApiClient
{
    private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, bool withAuth)
    {
        if (!withAuth) return await httpClient.SendAsync(request);

        var token = await sessionStorageService.GetItemAsStringAsync("token");

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.SendAsync(request);
    }

    private static HttpRequestMessage CreateHttpRequest(HttpMethod method, string uri, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (content is not null) request.Content = content;
        return request;
    }

    public Task<HttpResponseMessage> GetWithAuthAsync(string uri) =>
        SendAsync(CreateHttpRequest(HttpMethod.Get, uri), true);

    public Task<HttpResponseMessage> GetAsync(string uri) =>
        SendAsync(CreateHttpRequest(HttpMethod.Get, uri), false);

    public Task<HttpResponseMessage> PostWithAuthAsync(string uri, HttpContent? content) => 
        SendAsync(CreateHttpRequest(HttpMethod.Post, uri, content), true);

    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent? content) => 
        SendAsync(CreateHttpRequest(HttpMethod.Post, uri, content), false);

    public Task<HttpResponseMessage> PutWithAuthAsync(string uri, HttpContent? content) =>
        SendAsync(CreateHttpRequest(HttpMethod.Put, uri, content), true);

    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent? content) => 
        SendAsync(CreateHttpRequest(HttpMethod.Put, uri, content), false);

    public Task<HttpResponseMessage> DeleteWithAuthAsync(string uri) =>
        SendAsync(CreateHttpRequest(HttpMethod.Delete, uri), true);

    public Task<HttpResponseMessage> DeleteAsync(string uri) =>
        SendAsync(CreateHttpRequest(HttpMethod.Delete, uri), false);

    private async Task<ApiResponse<T>> HandleJsonRequestAsync<T>(HttpMethod method, string uri, HttpContent? content,
        bool expectContent, bool withAuth)
    {
        ApiResponse<T> apiResponse = new();

        try
        {
            var response = await SendAsync(CreateHttpRequest(method, uri, content), withAuth);

            apiResponse.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                if (expectContent)
                {
                    apiResponse.Data = await response.Content.ReadFromJsonAsync<T>();
                    if (apiResponse.Data is null && apiResponse.StatusCode != HttpStatusCode.NoContent)
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.ErrorMessage = "Server response unexpectedly empty";
                        return apiResponse;
                    }
                }

                apiResponse.IsSuccess = true;
                return apiResponse;
            }

            //!IsSuccessStatusCode
            if (apiResponse.StatusCode == HttpStatusCode.Unauthorized && withAuth)
                navigationManager.NavigateTo("/login");
            apiResponse.IsSuccess = false;
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
            apiResponse.ErrorMessage = errorResponse?.ErrorMessage ?? "Unexpected error";
            return apiResponse;
        }
        catch (JsonException)
        {
            apiResponse.IsSuccess = false;
            apiResponse.ErrorMessage = "Error parsing server response";
            return apiResponse;
        }
        catch (HttpRequestException)
        {
            apiResponse.IsSuccess = false;
            apiResponse.ErrorMessage = "Network error. Check connection";
            return apiResponse;
        }
        catch (Exception)
        {
            apiResponse.IsSuccess = false;
            apiResponse.ErrorMessage = "Unexpected error";
            return apiResponse;
        }
    }

    private Task<ApiResponse<T>> HandleWithContentAsync<T>(HttpMethod method, string uri, HttpContent? content,
        bool withAuth)
    {
        return HandleJsonRequestAsync<T>(method, uri, content, true, withAuth);
    }

    private async Task<ApiResponse> HandleWithoutContentAsync(HttpMethod method, string uri, HttpContent? content,
        bool withAuth)
    {
        var apiResponse = await HandleJsonRequestAsync<object>(method, uri, content, false, withAuth);
        return new ApiResponse()
        {
            IsSuccess = apiResponse.IsSuccess,
            ErrorMessage = apiResponse.ErrorMessage,
            StatusCode = apiResponse.StatusCode
        };
    }

    public Task<ApiResponse<T>> HandleJsonPostWithAuthAsync<T>(string uri, HttpContent? content) =>
        HandleWithContentAsync<T>(HttpMethod.Post, uri, content, true);

    public Task<ApiResponse<T>> HandleJsonPostAsync<T>(string uri, HttpContent? content) =>
        HandleWithContentAsync<T>(HttpMethod.Post, uri, content, false);

    public Task<ApiResponse> HandlePostWithAuthAsync(string uri, HttpContent? content) =>
        HandleWithoutContentAsync(HttpMethod.Post, uri, content, true);

    public Task<ApiResponse> HandlePostAsync(string uri, HttpContent? content) =>
        HandleWithoutContentAsync(HttpMethod.Post, uri, content, false);

    public Task<ApiResponse<T>> HandleJsonGetWithAuthAsync<T>(string uri) =>
        HandleWithContentAsync<T>(HttpMethod.Get, uri, null, true);


    public Task<ApiResponse<T>> HandleJsonGetAsync<T>(string uri) =>
        HandleWithContentAsync<T>(HttpMethod.Get, uri, null, false);

    public Task<ApiResponse> HandleGetWithAuthAsync(string uri) =>
        HandleWithoutContentAsync(HttpMethod.Get, uri, null, true);

    public Task<ApiResponse> HandleGetAsync(string uri) =>
        HandleWithoutContentAsync(HttpMethod.Get, uri, null, false);

    public Task<ApiResponse<T>> HandleJsonPutWithAuthAsync<T>(string uri, HttpContent? content) =>
        HandleWithContentAsync<T>(HttpMethod.Put, uri, content, true);

    public Task<ApiResponse<T>> HandleJsonPutAsync<T>(string uri, HttpContent? content) =>
        HandleWithContentAsync<T>(HttpMethod.Put, uri, content, false);

    public Task<ApiResponse> HandlePutWithAuthAsync(string uri, HttpContent? content) =>
    HandleWithoutContentAsync(HttpMethod.Put, uri, content, true);

    public Task<ApiResponse> HandlePutAsync(string uri, HttpContent? content) =>
        HandleWithoutContentAsync(HttpMethod.Put, uri, content, false);

    public Task<ApiResponse> HandleDeleteWithAuthAsync(string uri) =>
        HandleWithoutContentAsync(HttpMethod.Delete, uri, null, true);
    
    public Task<ApiResponse> HandleDeleteAsync(string uri) =>
        HandleWithoutContentAsync(HttpMethod.Delete, uri, null, false);
}