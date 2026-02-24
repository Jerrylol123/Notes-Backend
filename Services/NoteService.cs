using NotesApi.Models;
using NotesApi.Models.DTOs;
using NotesApi.Repositories;

namespace NotesApi.Services;

public interface INoteService
{
    Task<IEnumerable<NoteResponseDto>> GetNotesAsync(int userId, NoteQueryParams queryParams);
    Task<NoteResponseDto?> GetNoteAsync(int id, int userId);
    Task<NoteResponseDto> CreateNoteAsync(int userId, CreateNoteDto dto);
    Task<NoteResponseDto?> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto);
    Task<bool> DeleteNoteAsync(int id, int userId);
}

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<IEnumerable<NoteResponseDto>> GetNotesAsync(int userId, NoteQueryParams queryParams)
    {
        var notes = await _noteRepository.GetByUserIdAsync(userId, queryParams);
        return notes.Select(MapToDto);
    }

    public async Task<NoteResponseDto?> GetNoteAsync(int id, int userId)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId);
        return note is null ? null : MapToDto(note);
    }

    public async Task<NoteResponseDto> CreateNoteAsync(int userId, CreateNoteDto dto)
    {
        var now = DateTime.UtcNow;
        var note = new Note
        {
            UserId = userId,
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = now,
            UpdatedAt = now
        };

        var id = await _noteRepository.CreateAsync(note);
        note.Id = id;
        return MapToDto(note);
    }

    public async Task<NoteResponseDto?> UpdateNoteAsync(int id, int userId, UpdateNoteDto dto)
    {
        var existing = await _noteRepository.GetByIdAsync(id, userId);
        if (existing is null) return null;

        existing.Title = dto.Title;
        existing.Content = dto.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateAsync(existing);
        return MapToDto(existing);
    }

    public async Task<bool> DeleteNoteAsync(int id, int userId)
    {
        return await _noteRepository.DeleteAsync(id, userId);
    }

    private static NoteResponseDto MapToDto(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };
}
