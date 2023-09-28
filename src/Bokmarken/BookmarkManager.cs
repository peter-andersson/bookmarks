using System.Transactions;
using Bokmarken.Data;

namespace Bokmarken;

public class BookmarkManager
{
    private readonly ILogger<BookmarkManager> _logger;
    private readonly Database _database;
    
    public BookmarkManager(ILogger<BookmarkManager> logger, Database database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<IResult> AddBookmark(HttpRequest request)
    {
        _logger.LogInformation("AddBookmark");
        var bookmark = await request.ReadFromJsonAsync<Bookmark>();
        
        if (bookmark is null)
        {
            _logger.LogError("Can't add bookmark, missing object");
            return Results.BadRequest("Missing bookmark object");
        }        
        
        if (string.IsNullOrWhiteSpace(bookmark.Url))
        {
            _logger.LogError("Can't add bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }
        
        var tags = await CreateTags(bookmark);
        var createdBookmark = await _database.CreateBookmark(bookmark, tags);
        
        return Results.Json(createdBookmark);
    }
    
    public async Task<IResult> DeleteBookmark(int id)
    {
        _logger.LogInformation("Delete bookmark with {id}", id);

        await _database.DeleteBookmark(id);

        return Results.Ok();
    }       
    
    public async Task<IResult> GetBookmark(int id)
    {
        _logger.LogInformation("Get bookmark with {id}", id);

        var bookmark = await _database.GetBookmark(id);
        
        return bookmark is null ? Results.NotFound() : Results.Json(bookmark);
    }    

    public async Task<IResult> GetBookmarks()
    {
        _logger.LogInformation("Get bookmarks");

        var bookmarks = await _database.GetBookmarks();     

        return Results.Json(bookmarks);        
    }
    
    public async Task<IResult> UpdateBookmark(HttpRequest request)
    {
        _logger.LogInformation("UpdateBookmark");
        var bookmark = await request.ReadFromJsonAsync<Bookmark>();
        
        if (bookmark is null)
        {
            _logger.LogError("Can't update bookmark, missing object");
            return Results.BadRequest("Missing bookmark object");
        }        
        
        if (string.IsNullOrWhiteSpace(bookmark.Url))
        {
            _logger.LogError("Can't update bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }

        var existing = await _database.GetBookmark(bookmark.Id);
        if (existing is null)
        {
            _logger.LogError("No bookmark with id {id}", bookmark.Id);
            return Results.NotFound();
        }

        var tags = await CreateTags(bookmark);
        await _database.UpdateBookmark(bookmark, tags);
        
        return await GetBookmark(bookmark.Id);
    }
    
    private async Task<List<Tag>> CreateTags(Bookmark bookmark)
    {
        if (bookmark.Tags.Count == 0)
        {
            return new List<Tag>();
        }
        
        var tmp = await _database.GetTags();
        var tags = tmp.ToList();
        var tagsToCreate = new List<Tag>();
        var tagsToConnect = new List<Tag>();

        foreach (var tag in bookmark.Tags)
        {
            var existingTag = tags.FirstOrDefault(t => t.Name == NormalizeTagName(tag.Name));
            if (existingTag is not null)
            {
                tagsToConnect.Add(existingTag);
            }
            else
            {
                tagsToCreate.Add(tag);
            }
        }
        
        foreach (var tag in tagsToCreate)
        {
            var newTag = await _database.CreateTag(NormalizeTagName(tag.Name));
            if (newTag is not null)
            {
                tagsToConnect.Add(newTag);
            }
        }

        return tagsToConnect;
    }

    private static string NormalizeTagName(string tagName)
    {
        return tagName.ToLowerInvariant().Replace(' ', '-');
    } 
}