using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;
using Shared.Responses;

namespace Core.Interfaces.BusinessLogic.Services;

public interface IUserService
{
    Task<ServiceResult<UserDto>> AddAsync(UserDto userDto);
    Task<ServiceResult<ChallengeResponse>> GenerateChallenge(string username);
    Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest);
}