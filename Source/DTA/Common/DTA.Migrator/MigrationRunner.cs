using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DTA.Migrator;

/// <summary>
/// DTA migration runner
/// </summary>
public static class MigrationRunner
{
    private const string VersioningTableName = "DtaMigrations";

    /// <summary>
    /// Apply the migrations
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    /// <param name="configureMigrations">The migrations to apply setup via configuration action</param>
    public static void ApplyMigrations(this IServiceCollection serviceCollection, Action<MigrationCollector> configureMigrations)
    {
        var collector = new MigrationCollector();
        configureMigrations(collector);

        using var connection = new NpgsqlConnection(collector.ConnectionString);
        connection.Open();

        var migrations = collector.Migrations.OrderBy(m => m.Version);

        foreach (var migration in migrations)
        {
            if (!NeedsToApplyMigration(migration.Version, collector.ConnectionString)) continue;

            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new NpgsqlCommand(migration.Up(), connection);
                cmd.ExecuteNonQuery();

                RecordMigrationAsApplied(collector.ConnectionString, migration.Version, migration.GetType().Name);

                transaction.Commit();
                Console.WriteLine($"Applied migration: {migration.Version}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Failed to apply migration: {migration.Version}, Error: {ex.Message}");
                connection.Close();
                throw;
            }
        }

        connection.Close();
    }

    /// <summary>
    /// Initiate the migrator
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    public static IServiceCollection InitiateMigrator(this IServiceCollection serviceCollection, string connectionString)
    {
        //Create versioning table if it doesn't exist
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand($"""
                                           CREATE TABLE IF NOT EXISTS "{VersioningTableName}" (
                                                       "Version" BIGINT PRIMARY KEY,
                                                       "Name" TEXT NOT NULL
                                                                                                        
                                                   );
                                           """, connection);
        cmd.ExecuteNonQuery();

        return serviceCollection;
    }

    /// <summary>
    /// Check if migration needs to be applied
    /// </summary>
    /// <param name="version">Migration version</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Returns true, when migration needs to be applied, false otherwise</returns>
    private static bool NeedsToApplyMigration(long version, string connectionString)
    {
        //Check if migration has already been applied
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand($"""
                                           SELECT 1 FROM "{VersioningTableName}" WHERE "Version" = {version};
                                           """, connection);
        var exists = cmd.ExecuteScalar() != null;

        connection.Close();
        return !exists;
    }

    /// <summary>
    /// Record migration as applied
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="version">Migration version</param>
    /// <param name="migrationName">Migration name</param>
    private static void RecordMigrationAsApplied(string connectionString, long version, string migrationName = "Migration")
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand($"""
                                           INSERT INTO "{VersioningTableName}" ("Version", "Name") VALUES ({version}, '{migrationName}');
                                           """, connection);
        cmd.ExecuteNonQuery();
        connection.Close();
    }
}

/// <summary>
/// Migration collector for options action
/// </summary>
public class MigrationCollector
{
    public List<DtaMigration> Migrations { get; } = [];
    public string ConnectionString { get; set; } = string.Empty;
}