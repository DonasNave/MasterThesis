namespace DTA.Migrator;

public abstract class DtaMigration(long version)
{
    private long InternalVersion { get; } = version;
    public long Version => InternalVersion;
    public abstract string Up();
    public abstract string Down();
}