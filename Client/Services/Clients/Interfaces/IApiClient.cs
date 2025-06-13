using Client.Models;

namespace Client.Services.Clients.Interfaces;

public interface IApiClient
{
    Task<HttpResponseMessage> GetWithAuthAsync(string uri);
    Task<HttpResponseMessage> GetAsync(string uri);
    Task<HttpResponseMessage> PostWithAuthAsync(string uri, HttpContent? content);
    Task<HttpResponseMessage> PostAsync(string uri, HttpContent? content);

    Task<ApiResponse<T>> HandleJsonPostWithAuthAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse<T>> HandleJsonPostAsync<T>(string uri, HttpContent? content);
    Task<ApiResponse> HandleJsonPostWithAuthAsync(string uri, HttpContent? content);
    Task<ApiResponse> HandleJsonPostAsync(string uri, HttpContent? content);

    Task<ApiResponse<T>> HandleJsonGetWithAuthAsync<T>(string uri);
    Task<ApiResponse<T>> HandleJsonGetAsync<T>(string uri);
    Task<ApiResponse> HandleJsonGetWithAuthAsync(string uri);
    Task<ApiResponse> HandleJsonGetAsync(string uri);
}