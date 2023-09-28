using Npgsql;

namespace Bokmarken.Data;

public interface IDatabaseMigration
{
    Task ApplyMigration(NpgsqlConnection connection);
}