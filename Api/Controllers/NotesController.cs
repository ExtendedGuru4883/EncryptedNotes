using AutoMapper;
using Core.Abstractions.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Dto.Requests;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController(INoteService noteService, IMapper mapper) : ControllerBase
{
    [HttpGet(nameof(GetAll))]
    [ProducesResponseType(typeof(List<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<NoteDto>>> GetAll()
    {
        return ServiceResultMapper.ToActionResult(await noteService.GetAllForCurrentUser());
    }
    
    [HttpPost(nameof(Add))]
    [ProducesResponseType(typeof(List<NoteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NoteDto>> Add([FromBody] AddNoteRequest addNoteRequest)
    {
        var noteDto = mapper.Map<NoteDto>(addNoteRequest);
        return ServiceResultMapper.ToActionResult(await noteService.AddAsyncToCurrentUser(noteDto));
    }
}