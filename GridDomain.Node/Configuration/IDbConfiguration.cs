namespace GridDomain.Node.Configuration
{
    public interface IDbConfiguration
    {
        string ReadModelConnectionString { get; }
        string LogsConnectionString { get; }
    }
}