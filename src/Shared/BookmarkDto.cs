namespace Bookmarks.Shared;

public record BookmarkDto(int Id, string Url, string? Title, string? Description, List<string> Tags);