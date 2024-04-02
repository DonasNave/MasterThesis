using FluentMigrator;

namespace FUS.Migrations;

[Migration(20240401161604)]
public class AddFileTable : Migration
{
    public override void Up()
    {
        // Create the "Files" table
        Create.Table("Files")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("FileName").AsString()
            .WithColumn("Content").AsBinary();
    }

    public override void Down()
    {
        // Optionally, define how to revert the migration
        Delete.Table("Files");
    }
}