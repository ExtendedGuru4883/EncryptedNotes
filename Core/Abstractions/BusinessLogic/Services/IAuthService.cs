using Shared.Dto.Requests;
using Shared.Dto.Requests.Auth;
using Shared.Dto.Responses;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface IAuthService
{
    Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username);
    Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest);
}