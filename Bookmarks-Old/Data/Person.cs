using System.ComponentModel.DataAnnotations;

namespace Bookmarks.Data;

public class Person
{
        public int PersonId { get; init; }

        [StringLength(50)]
        public required string ApplicationUserId { get; init; }

        public ICollection<Bookmark> Bookmarks { get; } = [];
        
        public ICollection<Tag> Tags { get; } = [];
}