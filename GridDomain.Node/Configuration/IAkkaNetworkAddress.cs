namespace GridDomain.Node.Configuration
{
    public interface IAkkaNetworkAddress
    {
        string Name { get; }
        string Host { get; }
        int PortNumber { get; }
    }
}