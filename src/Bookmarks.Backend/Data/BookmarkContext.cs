using Bookmarks.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookmarks.Data;

#nullable disable

public class BookmarkContext : DbContext
{
    public BookmarkContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Tag> Tags { get; set; }
}