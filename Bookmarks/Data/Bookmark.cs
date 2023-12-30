using System.ComponentModel.DataAnnotations;

namespace Bookmarks.Data;

public sealed class Bookmark
{
    public int BookmarkId { get; init; }

    [StringLength(4096)]
    public required string Url { get; init; }

    [StringLength(256)]
    public string? Title { get; init; }

    [StringLength(1024)]
    public string? Description { get; init; }

    public ICollection<Tag> Tags { get; } = [];

    [Required]
    [StringLength(50)]
    public string UserId { get; init; } = null!;
    
    public ApplicationUser? User { get; init; }
}