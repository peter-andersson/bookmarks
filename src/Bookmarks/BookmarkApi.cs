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
    
    // TODO: Toast service in frontent
    
    private static async Task<IResult> AddBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, HttpRequest request, CancellationToken cancellationToken)
    {
        var bookmarkDto = await request.ReadFromJsonAsync<BookmarkDto>(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(bookmarkDto?.Url))
        {
            LogMessages.AddBookmarkMissingUrl(logger);
            return Results.BadRequest("Bookmark is missing url");
        }

        var bookmark = await dbContext.Bookmarks.Where(b => b.Url == bookmarkDto.Url).FirstOrDefaultAsync(cancellationToken);
        if (bookmark is not null)
        {
            LogMessages.AddBookmarkAlreadyExists(logger, bookmarkDto.Url);
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
    
    private static async Task<IResult> DeleteBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, [FromRoute]int id, CancellationToken cancellationToken)
    {
        var bookmark = await dbContext.Bookmarks.Include(b => b.Tags).Where(b => b.BookmarkId == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (bookmark is null)
        {
            LogMessages.BookmarkNotFound(logger, id);
            return Results.NotFound();
        }
        
        await dbContext.Bookmarks.Where(b => b.BookmarkId == id).ExecuteDeleteAsync(cancellationToken);

        return Results.Ok(ModelConverter.ConvertToDto(bookmark));
    }

    private static async Task<IResult> GetBookmarks(BookmarkContext dbContext, CancellationToken cancellationToken)
    {
        var bookmarks = await dbContext.Bookmarks.Include(b => b.Tags).OrderBy(b => b.Title).ToListAsync(cancellationToken);     

        return Results.Json(ModelConverter.ConvertToDto(bookmarks));      
    }
    
    private static async Task<IResult> GetBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, [FromRoute]int id, CancellationToken cancellationToken)
    {
        var bookmark = await dbContext.Bookmarks
            .Where(b => b.BookmarkId == id)
            .Include(b => b.Tags)
            .FirstOrDefaultAsync(cancellationToken);

        if (bookmark is not null)
        {
            return Results.Json(ModelConverter.ConvertToDto(bookmark));
        }
        
        LogMessages.BookmarkNotFound(logger, id);
        return Results.NotFound();
    }

    private static async Task<IResult> GetTags(BookmarkContext dbContext, CancellationToken cancellationToken)
    {
        var tags = await dbContext.Tags.OrderBy(t => t.Name).ToListAsync(cancellationToken);     

        return Results.Json(ModelConverter.ConvertToDto(tags));        
    }
    
    private static async Task<IResult> UpdateBookmark(BookmarkContext dbContext, ILogger<BookmarkApi> logger, HttpRequest request, CancellationToken cancellationToken)
    {
        var bookmarkDto = await request.ReadFromJsonAsync<BookmarkDto>(cancellationToken);
        
        if (string.IsNullOrWhiteSpace(bookmarkDto?.Url))
        {
            LogMessages.UpdateBookmarkMissingUrl(logger);
            return Results.BadRequest("Bookmark is missing url");
        }

        var existing = await dbContext.Bookmarks.Where(b => b.BookmarkId == bookmarkDto.Id).Include(b => b.Tags).FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
        {
            LogMessages.BookmarkNotFound(logger, bookmarkDto.Id);
            return Results.NotFound();
        }

        await HandleTags(dbContext, existing, bookmarkDto, cancellationToken);

        existing.Title = bookmarkDto.Title;
        existing.Description = bookmarkDto.Description;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await GetBookmark(dbContext, logger, existing.BookmarkId, cancellationToken);
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