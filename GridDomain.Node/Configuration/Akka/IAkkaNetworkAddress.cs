namespace GridDomain.Node.Configuration.Akka
{
    public interface IAkkaNetworkAddress
    {
        string SystemName { get; }
        string Host { get; }
        int PortNumber { get; }
    }
}