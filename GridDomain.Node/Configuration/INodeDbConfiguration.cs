namespace GridDomain.Node.Configuration
{
    public interface INodeDbConfiguration
    {
        string SnapshotConnectionString { get; }
        string JournalConnectionString { get; }
        string MetadataTableName { get; }
        string JournalTableName { get; }
        int JornalConnectionTimeoutSeconds { get; }
        int SnapshotsConnectionTimeoutSeconds { get; }
        string SnapshotTableName { get; }

        string SchemaName { get; }
    }
}