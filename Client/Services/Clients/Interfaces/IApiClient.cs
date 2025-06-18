using Client.Models;
using Client.Models.Responses;

namespace Client.Services.Clients.Interfaces;

public interface IApiClient
{
    Task<HttpResponseMessage> GetWithAuthAsync(string uri);
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<HttpResponseMessage> PostWithAuthAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> PostAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> PutWithAuthAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> PutAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> DeleteWithAuthAsync(string uri);
    Task<HttpResponseMessage> DeleteAsync(string uri);

    Task<ApiResponse<T>> HandleJsonPostWithAuthAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse<T>> HandleJsonPostAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse> HandlePostWithAuthAsync(string uri, HttpContent? content);
    Task<ApiResponse> HandlePostAsync(string uri, HttpContent? content);

    Task<ApiResponse<T>> HandleJsonGetWithAuthAsync<T>(string uri);
    Task<ApiResponse<T>> HandleJsonGetAsync<T>(string uri);
    Task<ApiResponse> HandleGetWithAuthAsync(string uri);
    Task<ApiResponse> HandleGetAsync(string uri);
    
    Task<ApiResponse<T>> HandleJsonPutWithAuthAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse<T>> HandleJsonPutAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse> HandlePutWithAuthAsync(string uri, HttpContent? content);
    Task<ApiResponse> HandlePutAsync(string uri, HttpContent? content);
    
    Task<ApiResponse> HandleDeleteWithAuthAsync(string uri);
    Task<ApiResponse> HandleDeleteAsync(string uri);
}