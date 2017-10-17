namespace GridDomain.Node.Persistence.Sql
{
    public interface ISqlNodeDbConfiguration
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