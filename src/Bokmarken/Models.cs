namespace Bokmarken;

public record Tag(int Id, string Name);

public record Bookmark(int Id, string Url, string? Title, string? Description, List<Tag> Tags);

public record Website(string? Title, string? Description);
