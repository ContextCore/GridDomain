namespace GridDomain.Node.Configuration
{
    public interface IDbConfiguration
    {
        string ReadModelConnectionString { get; }
        string EventStoreConnectionString { get; }
        string LogsConnectionString { get; }
    }

    public interface IAkkaDbConfiguration
    {
        string SnapshotConnectionString { get; }
        string JournalConnectionString { get; }
    }
}