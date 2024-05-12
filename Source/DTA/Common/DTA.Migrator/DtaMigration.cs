namespace DTA.Migrator;

/// <summary>
/// Base class for a DTA migration
/// </summary>
/// <param name="version">The version of the migration</param>
public abstract class DtaMigration(long version)
{
    private long InternalVersion { get; } = version;

    /// <summary>
    /// Gets the version of the migration
    /// </summary>
    public long Version => InternalVersion;

    /// <summary>
    /// Upgrade migration meant to apply the changes to the database
    /// </summary>
    public abstract string Up();

    /// <summary>
    /// Downgrade migration meant to revert the changes made by the upgrade migration
    public abstract string Down();
}