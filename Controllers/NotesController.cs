using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.Models.DTOs;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetNotes([FromQuery] NoteQueryParams queryParams)
    {
        var notes = await _noteService.GetNotesAsync(GetUserId(), queryParams);
        return Ok(notes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetNote(int id)
    {
        var note = await _noteService.GetNoteAsync(id, GetUserId());
        if (note is null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await _noteService.CreateNoteAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await _noteService.UpdateNoteAsync(id, GetUserId(), dto);
        if (note is null) return NotFound();
        return Ok(note);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var deleted = await _noteService.DeleteNoteAsync(id, GetUserId());
        if (!deleted) return NotFound();
        return NoContent();
    }
}
