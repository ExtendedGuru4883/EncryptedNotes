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
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Signup([FromBody] SignupRequest signupRequest)
    {
        var userDto = mapper.Map<UserDto>(signupRequest);
        return ServiceResultMapper.ToActionResult(await userService.AddAsync(userDto));
    }

    [HttpGet]
    [Route(nameof(Challenge))]
    [ProducesResponseType<ChallengeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChallengeResponse>> Challenge([FromQuery][Required] string username)
    {
        return ServiceResultMapper.ToActionResult(await authService.GenerateChallenge(username));
    }

    [HttpPost]
    [Route(nameof(Login))]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        return ServiceResultMapper.ToActionResult(await authService.Login(loginRequest));
    }
}