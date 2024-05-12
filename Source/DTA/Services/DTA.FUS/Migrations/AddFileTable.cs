using DTA.Migrator;

namespace DTA.FUS.Migrations;

/// <summary>
/// Migration to add the Files table
/// </summary>
public class AddFileTable(long version) : DtaMigration(version)
{
    /// <summary>
    /// Upgrades the database by creating the Files table
    /// </summary>
    public override string Up() =>
        """
        CREATE TABLE "Files" (
            "Id" SERIAL PRIMARY KEY,
            "FileName" TEXT NOT NULL,
            "Content" BYTEA NOT NULL
        );
        """;

    /// <summary>
    /// Downgrades the database by dropping the Files table
    /// </summary>
    public override string Down() =>
        """DROP TABLE IF EXISTS "Files";""";
}