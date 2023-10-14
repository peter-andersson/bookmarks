using Bookmarks.Data;
using Bookmarks.Data.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookmarks;

internal sealed class BookmarkApi
{
    public static void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/bookmark");
        group.MapGet("/", GetBookmarks);
        group.MapGet("/{id:int}", GetBookmark);
        group.MapPost("/", AddBookmark);
        group.MapPut("/", UpdateBookmark);
        group.MapDelete("/{id:int}", DeleteBookmark);
        group.MapPost("/info", LoadInfo);
        group.MapGet("/tags", GetTags);
    }
    
    private static async Task<IResult> AddBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, HttpRequest request, CancellationToken cancellationToken)
    {
        var bookmarkDto = await request.ReadFromJsonAsync<BookmarkDto>(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(bookmarkDto?.Url))
        {
            logger.LogError("Can't add bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }

        var bookmark = await dbContext.Bookmarks.Where(b => b.Url == bookmarkDto.Url).FirstOrDefaultAsync(cancellationToken);
        if (bookmark is not null)
        {
            logger.LogError("Can't add bookmark, already have bookmark with url {url}", bookmarkDto.Url);
            return Results.Conflict();
        }

        bookmark = new Bookmark()
        {
            Url = bookmarkDto.Url,
            Title = bookmarkDto.Title,
            Description = bookmarkDto.Description
        };

        await HandleTags(dbContext, bookmark, bookmarkDto, cancellationToken);

        dbContext.Bookmarks.Add(bookmark);
        await dbContext.SaveChangesAsync(cancellationToken);
       
        return Results.Json(ModelConverter.ConvertToDto(bookmark));
    }
    
    private static async Task<IResult> DeleteBookmark(BookmarkContext dbContext, CancellationToken cancellationToken, [FromRoute]int id)
    {
        await dbContext.Bookmarks.Where(b => b.BookmarkId == id).ExecuteDeleteAsync(cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> GetBookmarks(BookmarkContext dbContext, CancellationToken cancellationToken)
    {
        var bookmarks = await dbContext.Bookmarks.Include(b => b.Tags).ToListAsync(cancellationToken);     

        return Results.Json(ModelConverter.ConvertToDto(bookmarks));      
    }
    
    private static async Task<IResult> GetBookmark(BookmarkContext dbContext, CancellationToken cancellationToken, [FromRoute]int id)
    {
        var bookmark = await dbContext.Bookmarks
            .Where(b => b.BookmarkId == id)
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(cancellationToken);
        
        return bookmark is null ? Results.NotFound() : Results.Json(ModelConverter.ConvertToDto((bookmark)));
    }

    private static async Task<IResult> GetTags(BookmarkContext dbContext, CancellationToken cancellationToken)
    {
        var tags = await dbContext.Tags.ToListAsync(cancellationToken);     

        return Results.Json(ModelConverter.ConvertToDto(tags));        
    }
    
    private static async Task<IResult> UpdateBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, HttpRequest request, CancellationToken cancellationToken)
    {
        var bookmarkDto = await request.ReadFromJsonAsync<BookmarkDto>(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(bookmarkDto?.Url))
        {
            logger.LogError("Can't update bookmark, missing url");
            return Results.BadRequest("Bookmark is missing url");
        }

        var existing = await dbContext.Bookmarks.Where(b => b.BookmarkId == bookmarkDto.Id).Include(b => b.Tags).FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
        {
            logger.LogError("No bookmark with id {id}", bookmarkDto.Id);
            return Results.NotFound();
        }

        await HandleTags(dbContext, existing, bookmarkDto, cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await GetBookmark(dbContext, cancellationToken, existing.BookmarkId);
    }    
    
    private static async Task HandleTags(BookmarkContext dbContext, Bookmark bookmark, BookmarkDto bookmarkDto,
        CancellationToken cancellationToken)
    {
        bookmark.Tags.Clear();
        
        var tags = await dbContext.Tags.ToListAsync(cancellationToken);
        foreach (var tagName in bookmarkDto.Tags)
        {
            var tag = tags.FirstOrDefault(t => t.Name == NormalizeTagName(tagName));
            if (tag is null)
            {
                var newTag = new Tag()
                {
                    Name = NormalizeTagName(tagName)
                };

                dbContext.Tags.Add(newTag);
                bookmark.Tags.Add(newTag);
            }
            else
            {
                bookmark.Tags.Add(tag);
            }
        }
    }
    
    private static async Task<IResult> LoadInfo(HttpRequest request, CancellationToken cancellationToken)
    {
        using var sr = new StreamReader(request.Body);
        var url = await sr.ReadToEndAsync(cancellationToken);
    
        if (string.IsNullOrWhiteSpace(url))
        {
            return Results.BadRequest("Missing url");
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Results.BadRequest("Not a valid url");
        }

        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(url, cancellationToken);

        var title = string.Empty;
        var titleNode = doc.DocumentNode.SelectSingleNode("//head/title");
        if (titleNode is not null)
        {
            title = System.Net.WebUtility.HtmlDecode(titleNode.InnerText);
        }

        var description = string.Empty;
        var descriptionNode = doc.DocumentNode.SelectSingleNode("//head/meta[@name='description']");
        if (descriptionNode is not null)
        {
            description = System.Net.WebUtility.HtmlDecode(descriptionNode.GetAttributeValue("content", string.Empty));
        }

        return Results.Json(new WebsiteDto(title, description));
    }

    private static string NormalizeTagName(string tagName)
    {
        return tagName.ToLowerInvariant();
    }     
}