using DTA.Migrator;

namespace DTA.FUS.Migrations;

public class AddFileTable(long version) : DtaMigration(version)
{
    public override string Up() =>
        """
        CREATE TABLE "Files" (
            "Id" SERIAL PRIMARY KEY,
            "FileName" TEXT NOT NULL,
            "Content" BYTEA NOT NULL
        );
        """;

    public override string Down() =>
        """DROP TABLE IF EXISTS "Files";""";
}