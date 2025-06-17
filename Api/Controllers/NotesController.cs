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
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    
    [HttpPut("{noteId:guid}")]
    [ProducesResponseType<NoteDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoteDto>> Update([FromBody] UpdateNoteRequest updateNoteRequest, Guid noteId)
    {
        var noteDto = mapper.Map<NoteDto>(updateNoteRequest) with { Id = noteId };
        return ServiceResultMapper.ToActionResult(await noteService.UpdateForCurrentUserAsync(noteDto));
    }
    
    [HttpDelete("{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ErrorResponseDto>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid noteId)
    {
        return ServiceResultMapper.ToActionResult(await noteService.DeleteByIdForCurrentUserAsync(noteId));
    }
}