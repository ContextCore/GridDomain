namespace GridDomain.Node.Configuration.Akka
{
    public interface IAkkaNetworkAddress
    {
        string SystemName { get; }
        string Host { get; }
        int PortNumber { get; }
    }

    public static class AkkaNetworkAdressExtensions
    {
        public static string ToRootSelectionPath(this IAkkaNetworkAddress adr)
        {
            return $"akka.tcp://{adr.SystemName}@{adr.Host}:{adr.PortNumber}/user";
        }

    }
}