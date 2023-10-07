namespace Bookmarks.Data;

public class DatabaseMigrationData
{
    public required int Version { get; init; }

    public required string Description { get; init; }

    public required IDatabaseMigration Migration { get; init; }
}