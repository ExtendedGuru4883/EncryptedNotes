using Core.Abstractions.BusinessLogic.Services;
using EncryptedNotes.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;

namespace EncryptedNotes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController(INoteService noteService) : ControllerBase
{
    [HttpGet(nameof(GetAll))]
    [ProducesResponseType(typeof(List<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<NoteDto>>> GetAll()
    {
        return ServiceResultMapper.ToActionResult(await noteService.GetAllForCurrentUser());
    }
}