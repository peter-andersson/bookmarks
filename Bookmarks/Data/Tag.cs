using System.ComponentModel.DataAnnotations;

namespace Bookmarks.Data;

public class Tag
{
    public int TagId { get; set; }

    [StringLength(128)]
    public required string Name { get; init; }

    public ICollection<Bookmark> Bookmarks { get; } = [];
    
    public int PersonId { get; init; }
    
    public Person? Person { get; init; }    
}