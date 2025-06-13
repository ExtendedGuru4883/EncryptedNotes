using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Responses;

namespace Core.Interfaces.BusinessLogic.Services;

public interface IAuthService
{
    Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username);
    Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest);
}