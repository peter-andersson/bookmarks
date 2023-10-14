namespace Bookmarks.Data.Models;

public class Bookmark
{
    public int BookmarkId { get; set; }

    public required string Url { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<Tag> Tags { get; } = new();
}