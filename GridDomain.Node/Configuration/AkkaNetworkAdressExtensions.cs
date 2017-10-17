namespace GridDomain.Node.Configuration
{
    public static class AkkaNetworkAdressExtensions
    {
        public static string ToRootSelectionPath(this NodeConfiguration adr)
        {
            return $"akka.tcp://{adr.Name}@{adr.Address.Host}:{adr.Address.PortNumber}/user";
        }
    }
}