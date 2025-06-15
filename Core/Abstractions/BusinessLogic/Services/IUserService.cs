using Shared.Dto;
using Shared.Results;

namespace Core.Abstractions.BusinessLogic.Services;

public interface IUserService
{
    Task<ServiceResult<UserDto>> AddAsync(UserDto userDto);
}