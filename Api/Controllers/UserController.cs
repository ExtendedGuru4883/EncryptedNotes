using Core.Abstractions.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Requests.User;
using Shared.Dto.Responses;
using Shared.Dto.Responses.User;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("api/[controller]/me")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpDelete()]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete()
    {
        return ServiceResultMapper.ToActionResult(await userService.DeleteCurrentAsync());
    }
    
    [HttpPut("username")]
    [ProducesResponseType<UpdateUsernameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UpdateUsernameResponse>> UpdateUsername([FromBody] UpdateUsernameRequest request)
    {
        return ServiceResultMapper.ToActionResult(await userService.UpdateUsernameForCurrentUserAsync(request));
    }
}