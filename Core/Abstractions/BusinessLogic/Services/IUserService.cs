using Shared.Dto;
using Shared.Dto.Requests.User;
using Shared.Dto.Responses.User;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface IUserService
{
    Task<ServiceResult<UserDto>> AddAsync(UserDto userDto);
    Task<ServiceResult<UpdateUsernameResponse>> UpdateUsernameForCurrentUserAsync(UpdateUsernameRequest request);
    Task<ServiceResult> DeleteCurrentAsync();
}