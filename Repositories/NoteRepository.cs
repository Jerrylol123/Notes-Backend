using Dapper;
using NotesApi.Data;
using NotesApi.Models;
using NotesApi.Models.DTOs;

namespace NotesApi.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetByUserIdAsync(int userId, NoteQueryParams queryParams);
    Task<Note?> GetByIdAsync(int id, int userId);
    Task<int> CreateAsync(Note note);
    Task<bool> UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id, int userId);
}

public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NoteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Note>> GetByUserIdAsync(int userId, NoteQueryParams queryParams)
    {
        using var connection = _connectionFactory.CreateConnection();

        var allowedSortColumns = new HashSet<string> { "createdat", "updatedat", "title" };
        var sortColumn = allowedSortColumns.Contains(queryParams.SortBy.ToLower())
            ? queryParams.SortBy.ToLower() switch
            {
                "createdat" => "CreatedAt",
                "updatedat" => "UpdatedAt",
                "title" => "Title",
                _ => "CreatedAt"
            }
            : "CreatedAt";

        var sortOrder = queryParams.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";

        var sql = $@"
            SELECT * FROM Notes
            WHERE UserId = @UserId
            {(string.IsNullOrWhiteSpace(queryParams.Search) ? "" : "AND (Title LIKE @Search OR Content LIKE @Search)")}
            ORDER BY {sortColumn} {sortOrder}";

        return await connection.QueryAsync<Note>(sql, new
        {
            UserId = userId,
            Search = $"%{queryParams.Search}%"
        });
    }

    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task<int> CreateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(@"
            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id
            VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt)",
            new { note.UserId, note.Title, note.Content, note.CreatedAt, note.UpdatedAt });
    }

    public async Task<bool> UpdateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(@"
            UPDATE Notes SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt
            WHERE Id = @Id AND UserId = @UserId",
            new { note.Title, note.Content, note.UpdatedAt, note.Id, note.UserId });
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(
            "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
        return rows > 0;
    }
}
