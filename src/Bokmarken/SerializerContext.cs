using System.Text.Json.Serialization;

namespace Bokmarken;

[JsonSerializable(typeof(Bookmark))]
[JsonSerializable(typeof(List<Bookmark>))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(List<Tag>))]
[JsonSerializable(typeof(Website))]
public partial class SerializerContext : JsonSerializerContext
{
}