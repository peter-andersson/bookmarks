using System.Diagnostics;
using Npgsql;
using NpgsqlTypes;

namespace Bookmarks.Data;

public sealed class Database
{
    private readonly ILogger<Database> _logger;
    private readonly string _connectionString;
    
    public Database(ILogger<Database> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("Postgres") ?? string.Empty;
    }

    public async Task<Bookmark?> CreateBookmark(Bookmark bookmark, List<Tag> tags)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO bookmark (title, url, description) VALUES (@Title, @Url, @Description) RETURNING id";
        command.Parameters.Add("@Url", NpgsqlDbType.Varchar).Value = bookmark.Url;
        command.Parameters.Add("@Title", NpgsqlDbType.Varchar).Value = string.IsNullOrWhiteSpace(bookmark.Title) ? DBNull.Value : bookmark.Title;
        command.Parameters.Add("@Description", NpgsqlDbType.Varchar).Value = string.IsNullOrWhiteSpace(bookmark.Description) ? DBNull.Value : bookmark.Description;
        var id =  Convert.ToInt32(await command.ExecuteScalarAsync());
        
        command.CommandText ="INSERT INTO bookmark_tag (bookmark_id, tag_id) VALUES (@BookmarkId, @TagId)";
        foreach (var tag in tags)
        {
            command.Parameters.Clear();
            command.Parameters.Add("@BookmarkId", NpgsqlDbType.Integer).Value = id;
            command.Parameters.Add("@TagId", NpgsqlDbType.Integer).Value = tag.Id;
            await command.ExecuteNonQueryAsync();
        }
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("Create bookmark with id {id} took {time} ms.", id, elapsedTime.TotalMilliseconds);

        return await GetBookmark(id);
    }
    
    public async Task<Tag?> CreateTag(string tagName)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        
        command.CommandText = "INSERT INTO tag (name) VALUES (@Name) RETURNING id, name";
        command.Parameters.Add("@Name", NpgsqlDbType.Varchar).Value = tagName;

        await using var reader = await command.ExecuteReaderAsync();
        Tag? result = null;
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);

            result = new Tag(id, name);
        }

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("Add new tag took {time} ms.", elapsedTime.TotalMilliseconds);
        
        return result;
    }

    public async Task DeleteBookmark(int id)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        
        command.CommandText = "DELETE FROM bookmark_tag WHERE bookmark_id = @Id";
        command.Parameters.Add("@Id", NpgsqlDbType.Integer).Value = id;
        await command.ExecuteNonQueryAsync();
        
        command.CommandText = "DELETE FROM bookmark WHERE id = @Id";
        await command.ExecuteNonQueryAsync();
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("Delete bookmark took {time} ms.", elapsedTime.TotalMilliseconds);
    }

    public async Task<Bookmark?> GetBookmark(int bookmarkId)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = 
            """
            SELECT
                a.id, a.url, a.title, a.description,
                c.id, c.name
            FROM bookmark a
            LEFT JOIN bookmark_tag b ON a.id  = b.bookmark_id
            LEFT JOIN tag c ON b.tag_id = c.id
            WHERE a.id = @Id
            """; 
        command.Parameters.Add("@Id", NpgsqlDbType.Integer).Value = bookmarkId;

        Bookmark? bookmark = null; 
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var url = reader.GetString(1);
            var title = reader.IsDBNull(2) ? null : reader.GetString(2);
            var description = reader.IsDBNull(3) ? null : reader.GetString(3);

            if (bookmark is null || (bookmark.Id != id))
            {
                bookmark = new Bookmark(id, url, title, description, new List<Tag>());
            }

            var tagId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
            var tagName = reader.IsDBNull(5) ? null : reader.GetString(5);

            if (tagId.HasValue && tagName is not null)
            {
                bookmark.Tags.Add(new Tag(tagId.Value, tagName));
            }
        }
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("GetBookmark with id {id} took {time} ms.", bookmarkId, elapsedTime.TotalMilliseconds);

        return bookmark;
    }

    public async Task<List<Bookmark>> GetBookmarks()
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = 
            """
            SELECT
                a.id, a.url, a.title, a.description,
                c.id, c.name
            FROM bookmark a
            LEFT JOIN bookmark_tag b ON a.id  = b.bookmark_id
            LEFT JOIN tag c ON b.tag_id = c.id
            """;            

        var result = new List<Bookmark>();

        Bookmark? bookmark = null; 
        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var url = reader.GetString(1);
            var title = reader.IsDBNull(2) ? null : reader.GetString(2);
            var description = reader.IsDBNull(3) ? null : reader.GetString(3);

            if (bookmark is null || (bookmark.Id != id))
            {
                bookmark = new Bookmark(id, url, title, description, new List<Tag>());
                result.Add(bookmark);
            }

            var tagId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
            var tagName = reader.IsDBNull(5) ? null : reader.GetString(5);

            if (tagId.HasValue && tagName is not null)
            {
                bookmark.Tags.Add(new Tag(tagId.Value, tagName));
            }
        }
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("GetBookmarks fetched {count} bookmarks in {time} ms.", result.Count, elapsedTime.TotalMilliseconds);

        return result;
    }
    
    public async Task<List<Tag>> GetTags()
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name FROM tag ORDER BY name";

        var result = new List<Tag>();

        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);

            result.Add(new Tag(id, name));
        }
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("GetTags fetched {count} tags in {time} ms.", result.Count, elapsedTime.TotalMilliseconds);

        return result;
    }

    public async Task UpdateBookmark(Bookmark bookmark, List<Tag> tags)
    {
        var startTime = Stopwatch.GetTimestamp();
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = connection.CreateCommand();
        command.CommandText = """
                              UPDATE bookmark
                              SET
                                  url = @Url,
                                  title = @Title,
                                  description = @Description
                              WHERE id = @Id
                              """;
        command.Parameters.Add("@Id", NpgsqlDbType.Integer).Value = bookmark.Id;
        command.Parameters.Add("@Url", NpgsqlDbType.Varchar).Value = bookmark.Url;
        command.Parameters.Add("@Title", NpgsqlDbType.Varchar).Value = string.IsNullOrWhiteSpace(bookmark.Title) ? DBNull.Value : bookmark.Title;
        command.Parameters.Add("@Description", NpgsqlDbType.Varchar).Value = string.IsNullOrWhiteSpace(bookmark.Description) ? DBNull.Value : bookmark.Description;
        await command.ExecuteNonQueryAsync();
        
        command.CommandText ="DELETE FROM bookmark_tag WHERE bookmark_id = @Id";
        command.Parameters.Clear();
        command.Parameters.Add("@Id", NpgsqlDbType.Integer).Value = bookmark.Id;
        await command.ExecuteNonQueryAsync();
        
        command.CommandText ="INSERT INTO bookmark_tag (bookmark_id, tag_id) VALUES (@BookmarkId, @TagId)";
        foreach (var tag in tags)
        {
            command.Parameters.Clear();
            command.Parameters.Add("@BookmarkId", NpgsqlDbType.Integer).Value = bookmark.Id;
            command.Parameters.Add("@TagId", NpgsqlDbType.Integer).Value = tag.Id;
            await command.ExecuteNonQueryAsync();
        }
        
        var elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogDebug("Updated bookmark with id {id} took {time} ms.", bookmark.Id, elapsedTime.TotalMilliseconds);        
    }
}