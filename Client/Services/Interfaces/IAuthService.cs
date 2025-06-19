using Client.Models.Results;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> SignupAsync(string username, string password);
}