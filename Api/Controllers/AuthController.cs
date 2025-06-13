using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Core.Interfaces.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Responses;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Route(nameof(Signup))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Signup([FromBody] SignupRequest signupRequest)
    {
        var userDto = mapper.Map<UserDto>(signupRequest);
        return ServiceResponseMapper.ToActionResult(await userService.AddAsync(userDto));
    }

    [HttpGet]
    [Route(nameof(GetChallenge))]
    [ProducesResponseType(typeof(ChallengeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChallengeResponse>> GetChallenge([FromQuery][Required] string username)
    {
        return ServiceResponseMapper.ToActionResult(await userService.GenerateChallenge(username));
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
        return ServiceResponseMapper.ToActionResult(await userService.Login(loginRequest));
    }
}