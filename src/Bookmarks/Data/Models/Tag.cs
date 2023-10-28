namespace Bookmarks.Data.Models;

public class Tag
{
    public int TagId { get; set; }

    public required string Name { get; set; }

    public List<Bookmark> Bookmarks { get; } = new();
}