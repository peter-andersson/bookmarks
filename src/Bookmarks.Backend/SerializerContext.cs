using System.Text.Json.Serialization;

namespace Bookmarks;

[JsonSerializable(typeof(BookmarkDto))]
[JsonSerializable(typeof(List<BookmarkDto>))]
[JsonSerializable(typeof(WebsiteDto))]
public partial class SerializerContext : JsonSerializerContext
{
}