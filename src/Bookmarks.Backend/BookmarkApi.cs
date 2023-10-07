using Bookmarks.Data;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Bookmarks;

internal static class BookmarkApi
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    public static void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/bookmark");
        group.MapGet("/", GetBookmarks);
        group.MapGet("/{id:int}", GetBookmark);
        group.MapPost("/", AddBookmark);
        group.MapPut("/", UpdateBookmark);
        group.MapDelete("/{id:int}", DeleteBookmark);
        group.MapPost("/info", async (HttpRequest request, WebSiteInfo webSiteInfo) => await webSiteInfo.LoadInfo(request));
        group.MapGet("/tags", GetTags);
    }
    
    private static async Task<IResult> AddBookmark([FromServices]Database database, HttpRequest request)
    {
        Logger.Debug("AddBookmark");
        var bookmark = await request.ReadFromJsonAsync<Bookmark>();
        
        if (bookmark is null)
        {
            Logger.Error("Can't add bookmark, missing object");
            return Results.BadRequest("Missing bookmark object");
        }        
        
        if (string.IsNullOrWhiteSpace(bookmark.Url))
        {
            Logger.Error("Can't add bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }
        
        var tags = await CreateTags(database, bookmark);
        var createdBookmark = await database.CreateBookmark(bookmark, tags);
        
        return Results.Json(createdBookmark);
    }    
    
    private static async Task<IResult> DeleteBookmark([FromServices]Database database, [FromRoute]int id)
    {
        Logger.Info("Delete bookmark with {id}", id);

        await database.DeleteBookmark(id);

        return Results.Ok();
    }

    private static async Task<IResult> GetBookmarks([FromServices]Database database)
    {
        Logger.Debug("Get bookmarks");

        var bookmarks = await database.GetBookmarks();     

        return Results.Json(bookmarks);      
    }
    
    private static async Task<IResult> GetBookmark([FromServices]Database database, [FromRoute]int id)
    {
        Logger.Debug("Get bookmark with {id}", id);

        var bookmark = await database.GetBookmark(id);
        
        return bookmark is null ? Results.NotFound() : Results.Json(bookmark);
    }

    private static async Task<IResult> GetTags([FromServices] Database database)
    {
        Logger.Debug("Get tags");

        var tags = await database.GetTags();     

        return Results.Json(tags);        
    }
    
    private static async Task<IResult> UpdateBookmark([FromServices]Database database, HttpRequest request)
    {
        Logger.Debug("UpdateBookmark");
        var bookmark = await request.ReadFromJsonAsync<Bookmark>();
        
        if (bookmark is null)
        {
            Logger.Error("Can't update bookmark, missing object");
            return Results.BadRequest("Missing bookmark object");
        }        
        
        if (string.IsNullOrWhiteSpace(bookmark.Url))
        {
            Logger.Error("Can't update bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }

        var existing = await database.GetBookmark(bookmark.Id);
        if (existing is null)
        {
            Logger.Error("No bookmark with id {id}", bookmark.Id);
            return Results.NotFound();
        }

        var tags = await CreateTags(database, bookmark);
        await database.UpdateBookmark(bookmark, tags);
        
        return await GetBookmark(database, bookmark.Id);
    }    
    
    private static async Task<List<Tag>> CreateTags(Database database, Bookmark bookmark)
    {
        if (bookmark.Tags.Count == 0)
        {
            return new List<Tag>();
        }
        
        var tmp = await database.GetTags();
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
            var newTag = await database.CreateTag(NormalizeTagName(tag.Name));
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