namespace GridDomain.Node.Akka {
    public static class NodeNetworkAddressExtensions
    {
        public static string ToTcpAddress(this NodeNetworkAddress conf)
        {
            return $"akka.tcp://{conf.Name}@{conf.Host}:{conf.PortNumber}";
        }
    }
}