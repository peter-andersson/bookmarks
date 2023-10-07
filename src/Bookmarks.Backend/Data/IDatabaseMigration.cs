using Npgsql;

namespace Bookmarks.Data;

internal interface IDatabaseMigration
{
    Task ApplyMigration(NpgsqlConnection connection);
}