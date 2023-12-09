using System.ComponentModel.DataAnnotations;

namespace Bookmarks.Server.Data;

public class Bookmark
{
    public int BookmarkId { get; init; }

    [StringLength(4096)]
    public required string Url { get; init; }

    [StringLength(256)]
    public string? Title { get; set; }

    [StringLength(1024)]
    public string? Description { get; set; }

    public ICollection<Tag> Tags { get; } = [];

    public int PersonId { get; init; }
    
    public Person? Person { get; set; }
}