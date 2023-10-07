using System.ComponentModel.DataAnnotations;

namespace Bookmarks;

internal sealed class BookmarkOptions
{
    public const string SectionName = "Bookmark";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}