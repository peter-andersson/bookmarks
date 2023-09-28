using Bokmarken.Data.Migrations;
using Npgsql;
using NpgsqlTypes;

namespace Bokmarken.Data;

public class DatabaseUpdate
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseUpdate> _logger;
    
    private int _maxVersion;
    private readonly Dictionary<int, DatabaseMigrationData> _migrations = new();
    
    // TODO: Read connection string from configuration
    public DatabaseUpdate(ILogger<DatabaseUpdate> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("Postgres") ?? string.Empty;

        SetupMigrations();
    }

    /// <summary>
    /// Ensure that the latest database version is applied.
    /// </summary>
    public async Task EnsureDatabaseVersion()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new Exception("Missing connection string");
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var currentVersion = 0;
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(version), 0) FROM version_info";
            currentVersion =  Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (PostgresException e)
        {
            if (!e.Message.Contains("42P01: relation \"version_info\" does not exist"))
            {
                throw;
            }
        }

        _logger.LogInformation("Database version {version}", currentVersion);
        while (currentVersion < _maxVersion)
        {
            currentVersion += 1;

            if (_migrations.TryGetValue(currentVersion, out var migration))
            {
                _logger.LogInformation("Update database to version {version}: {description}", migration.Version, migration.Description);
                await migration.Migration.ApplyMigration(connection);
                await AddToVersionTable(connection, migration.Version, migration.Description);
            }
            else
            {
                _logger.LogError("Missing database migration for version {version}", currentVersion);
                throw new Exception($"Missing database migration for version {currentVersion}");
            }
        }
    }

    private static async Task AddToVersionTable(NpgsqlConnection connection, int version, string description)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "INTO version_info (version, applied, description) VALUES (@Version, @AppliedAt, @Description)";
        command.Parameters.Add("@Version", NpgsqlDbType.Integer).Value = version;
        command.Parameters.Add("@AppliedAt", NpgsqlDbType.Timestamp).Value = DateTime.UtcNow;
        command.Parameters.Add("@Description", NpgsqlDbType.Varchar).Value = description;

        await command.ExecuteNonQueryAsync();
    }

    private void SetupMigrations()
    {
        _migrations.Add(1, new DatabaseMigrationData()
        {
            Version = 1,
            Description = "Initial create of database",
            Migration = new Migration001()
        });
        
        _maxVersion = _migrations.Keys.Max();
    }
}