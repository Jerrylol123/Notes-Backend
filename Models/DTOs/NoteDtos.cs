using System.ComponentModel.DataAnnotations;

namespace NotesApi.Models.DTOs;

public class CreateNoteDto
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class UpdateNoteDto
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class NoteResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class NoteQueryParams
{
    public string? Search { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
}
