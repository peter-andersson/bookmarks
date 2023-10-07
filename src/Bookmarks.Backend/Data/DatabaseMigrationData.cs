namespace Bookmarks.Data;

internal record DatabaseMigrationData(int Version, string Description, IDatabaseMigration Migration);