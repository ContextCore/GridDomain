namespace GridDomain.Node.Configuration.Persistence
{
    public interface IDbConfiguration
    {
        string ReadModelConnectionString { get; }
        string LogsConnectionString { get; }
    }
}