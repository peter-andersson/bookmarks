using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookmarks.Data;

#nullable disable

public class BookmarkContext : IdentityDbContext<IdentityUser>
{
    public BookmarkContext(DbContextOptions<BookmarkContext> options) :
        base(options) { }
    
    public DbSet<Bookmark> Bookmarks { get; init; }
    
    public DbSet<Person> Persons { get; init; }
    
    public DbSet<Tag> Tags { get; init; }
}