namespace GridDomain.Node.Configuration
{
    public interface IAkkaNetworkConfiguration
    {
        string Name { get; }
        string Host { get; }
        int PortNumber { get; }
    }
}