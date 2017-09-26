namespace GridDomain.Node.Configuration
{
    public static class AkkaNetworkAdressExtensions
    {
        public static string ToRootSelectionPath(this INodeNetworkAddress adr)
        {
            return $"akka.tcp://{adr.SystemName}@{adr.Host}:{adr.PortNumber}/user";
        }
    }
}