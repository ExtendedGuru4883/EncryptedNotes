using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete()
    {
        return ServiceResultMapper.ToActionResult(await userService.DeleteCurrentAsync());
    }
}