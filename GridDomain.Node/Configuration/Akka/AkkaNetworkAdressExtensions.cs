namespace GridDomain.Node.Configuration.Akka
{
    public static class AkkaNetworkAdressExtensions
    {
        public static string ToRootSelectionPath(this IAkkaNetworkAddress adr)
        {
            return $"akka.tcp://{adr.SystemName}@{adr.Host}:{adr.PortNumber}/user";
        }
    }
}