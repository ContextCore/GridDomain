namespace GridDomain.Node.Configuration.Akka
{
    public interface IAkkaDbConfiguration
    {
        string SnapshotConnectionString { get; }
        string JournalConnectionString { get; }
        string MetadataTableName { get; }
        string JournalTableName { get; }
        int JornalConnectionTimeoutSeconds { get; }
        string SnapshotTableName { get; }
    }
}