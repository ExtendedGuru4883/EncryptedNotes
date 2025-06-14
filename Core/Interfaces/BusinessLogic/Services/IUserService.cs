using Shared.Dto;
using Shared.Results;

namespace Core.Interfaces.BusinessLogic.Services;

public interface IUserService
{
    Task<ServiceResult<UserDto>> AddAsync(UserDto userDto);
}