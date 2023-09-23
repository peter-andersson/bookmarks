using System.Data;
using Bokmarken.Database.Migrations;
using Dapper;
using Npgsql;

namespace Bokmarken.Database;

public class DatabaseUpdate
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseUpdate> _logger;
    
    private int _maxVersion;
    private Dictionary<int, DatabaseMigrationData> _migrations = new();
    
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
            currentVersion =
                await connection.QuerySingleAsync<int>("SELECT COALESCE(MAX(version), 0) FROM version_info");
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

    private static async Task AddToVersionTable(IDbConnection connection, int version, string description)
    {
        const string sql = "INSERT INTO version_info (version, applied, description) VALUES (@Version, @AppliedAt, @Description)";

        var data = new { Version = version, AppliedAt = DateTime.UtcNow, Description = description };
        await connection.ExecuteAsync(sql, data);
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