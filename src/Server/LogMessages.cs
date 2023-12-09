namespace Bookmarks.Server;

public static partial class LogMessages
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Error, Message = "Can't add bookmark, missing url")]
    public static partial void AddBookmarkMissingUrl(ILogger logger);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error,
        Message = "Can't add bookmark, already have bookmark with url {url}")]
    public static partial void AddBookmarkAlreadyExists(ILogger logger, string url);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Can't update bookmark, missing url")]
    public static partial void UpdateBookmarkMissingUrl(ILogger logger);
    
    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "No bookmark with id {id}")]
    public static partial void BookmarkNotFound(ILogger logger, int id);    
}