using System.ComponentModel.DataAnnotations;

namespace Bookmarks.Data;

public sealed class Tag
{
    public int TagId { get; set; }

    [StringLength(128)]
    public required string Name { get; init; }

    public ICollection<Bookmark> Bookmarks { get; } = [];
    
    [Required]
    [StringLength(50)]
    public string UserId { get; init; } = null!;
    
    public ApplicationUser? User { get; init; }    
}