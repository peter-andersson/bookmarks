using Bookmarks.Server.Data;
using Bookmarks.Shared;

namespace Bookmarks.Server;

internal static class ModelConverter
{
    public static BookmarkDto ConvertToDto(Bookmark bookmark)
    {
        var result = new BookmarkDto(
            bookmark.BookmarkId,
            bookmark.Url,
            bookmark.Title,
            bookmark.Description,
            []);

        foreach (var tag in bookmark.Tags)
        {
            result.Tags.Add(tag.Name);
        }
        
        result.Tags.Sort();

        return result;
    }

    public static List<BookmarkDto> ConvertToDto(List<Bookmark> bookmarks)
    {
        var result = new List<BookmarkDto>(bookmarks.Count);
        result.AddRange(bookmarks.Select(ConvertToDto));
        return result;
    }

    public static List<string> ConvertToDto(List<Tag> tags)
    {
        var result = new List<string>(tags.Count);
        result.AddRange(tags.Select(t => t.Name));
        return result;
    }
}