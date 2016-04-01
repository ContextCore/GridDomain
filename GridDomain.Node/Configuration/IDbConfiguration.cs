namespace GridDomain.Node.Configuration
{
    public interface IDbConfiguration
    {
        string ReadModelConnectionString { get; }
        string EventStoreConnectionString { get; }
        string LogsConnectionString { get; }
    }
}