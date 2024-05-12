using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DTA.Extensions;

/// <summary>
/// Extensions for the database
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Ensure the database exists
    /// </summary>
    /// <param name="serviceCollection">The application service collection</param>
    /// <param name="connectionString">The connection string to database</param>
    public static IServiceCollection EnsureDatabaseExists(this IServiceCollection serviceCollection, string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var dbName = builder.Database;

        // Connect to the default database
        builder.Database = "postgres";

        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{dbName}'", connection);
        var exists = cmd.ExecuteScalar() != null;

        if (exists) return serviceCollection;

        using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection);
        createCmd.ExecuteNonQuery();

        return serviceCollection;
    }
}