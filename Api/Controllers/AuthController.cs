using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService, IAuthService authService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route(nameof(Signup))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Signup([FromBody] SignupRequest signupRequest)
    {
        var userDto = mapper.Map<UserDto>(signupRequest);
        return ServiceResultMapper.ToActionResult(await userService.AddAsync(userDto));
    }

    [HttpGet]
    [Route(nameof(Challenge))]
    [ProducesResponseType(typeof(ChallengeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChallengeResponse>> Challenge([FromQuery][Required] string username)
    {
        return ServiceResultMapper.ToActionResult(await authService.GenerateChallenge(username));
    }

    [HttpPost]
    [Route(nameof(Login))]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        return ServiceResultMapper.ToActionResult(await authService.Login(loginRequest));
    }
}