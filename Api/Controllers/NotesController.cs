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
public class NotesController(INoteService noteService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PaginatedResponse<NoteDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedResponse<NoteDto>>> Get(
        [FromQuery] [Required] PaginatedNotesRequest request)
    {
        return ServiceResultMapper.ToActionResult(await noteService.GetPageForCurrentUser(request));
    }

    [HttpPost]
    [ProducesResponseType<NoteDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NoteDto>> Add([FromBody] AddNoteRequest addNoteRequest)
    {
        //Note: if the user doesn't exist in the database anymore but the request is made with a still valid JWT
        //The add operation will fail for foreign key violation and the error handling middleware will return 500
        var noteDto = mapper.Map<NoteDto>(addNoteRequest);
        return ServiceResultMapper.ToActionResult(await noteService.AddAsyncToCurrentUser(noteDto));
    }
}