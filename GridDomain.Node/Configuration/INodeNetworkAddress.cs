namespace GridDomain.Node.Configuration
{
    public interface INodeNetworkAddress
    {
        string Host { get; }
        string PublicHost { get; }
        int PortNumber { get; }
        bool EnforceIpVersion { get; }
    }
}