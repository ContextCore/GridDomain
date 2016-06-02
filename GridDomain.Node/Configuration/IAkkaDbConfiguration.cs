namespace GridDomain.Node.Configuration
{
    public interface IAkkaDbConfiguration
    {
        string SnapshotConnectionString { get; }
        string JournalConnectionString { get; }
        string MetadataTableName { get; }
        string JournalTableName { get; }
        string SnapshotTableName { get; }
    }
}