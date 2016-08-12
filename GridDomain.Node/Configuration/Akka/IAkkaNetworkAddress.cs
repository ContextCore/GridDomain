namespace GridDomain.Node.Configuration.Akka
{
    public interface IAkkaNetworkAddress
    {
        string SystemName { get; }
        string Host { get; }
        string PublicHost { get; }
        int PortNumber { get; }
        bool EnforceIpVersion { get; }
    }
}