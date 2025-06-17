using Client.Models;

namespace Client.Services.Clients.Interfaces;

public interface IApiClient
{
    Task<HttpResponseMessage> GetWithAuthAsync(string uri);
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<HttpResponseMessage> PostWithAuthAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> PostAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> DeleteWithAuthAsync(string uri);
    Task<HttpResponseMessage> DeleteAsync(string uri);

    Task<ApiResponse<T>> HandleJsonPostWithAuthAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse<T>> HandleJsonPostAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse> HandleJsonPostWithAuthAsync(string uri, HttpContent? content);
    Task<ApiResponse> HandleJsonPostAsync(string uri, HttpContent? content);

    Task<ApiResponse<T>> HandleGetWithAuthAsync<T>(string uri);
    Task<ApiResponse<T>> HandleGetAsync<T>(string uri);
    Task<ApiResponse> HandleGetWithAuthAsync(string uri);
    Task<ApiResponse> HandleGetAsync(string uri);
    
    Task<ApiResponse> HandleDeleteWithAuthAsync(string uri);
    Task<ApiResponse> HandleDeleteAsync(string uri);
}