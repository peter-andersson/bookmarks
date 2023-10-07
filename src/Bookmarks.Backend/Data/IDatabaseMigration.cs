using Npgsql;

namespace Bookmarks.Data;

public interface IDatabaseMigration
{
    Task ApplyMigration(NpgsqlConnection connection);
}