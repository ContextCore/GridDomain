namespace GridDomain.Node.Configuration
{
    public interface INodeNetworkAddress
    {
        string SystemName { get; }
        string Host { get; }
        string PublicHost { get; }
        int PortNumber { get; }
        bool EnforceIpVersion { get; }
    }
}