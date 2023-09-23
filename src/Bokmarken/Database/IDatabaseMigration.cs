using System.Data;

namespace Bokmarken.Database;

public interface IDatabaseMigration
{
    Task ApplyMigration(IDbConnection connection);
}