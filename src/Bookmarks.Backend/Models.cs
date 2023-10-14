namespace Bookmarks;

public record BookmarkDto(int Id, string Url, string? Title, string? Description, List<string> Tags);

public record WebsiteDto(string? Title, string? Description);
