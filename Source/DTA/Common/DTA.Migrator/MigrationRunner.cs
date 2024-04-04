using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DTA.Migrator;


public static class MigrationRunner
{
    private const string VersioningTableName = "DtaMigrations";

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

public class MigrationCollector
{
    public List<DtaMigration> Migrations { get; } = [];
    public string ConnectionString { get; set; } = string.Empty;
}