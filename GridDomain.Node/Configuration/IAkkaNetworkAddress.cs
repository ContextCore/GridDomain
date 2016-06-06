namespace GridDomain.Node.Configuration
{
    public interface IAkkaNetworkAddress
    {
        string SystemName { get; }
        string Host { get; }
        int PortNumber { get; }
    }
}