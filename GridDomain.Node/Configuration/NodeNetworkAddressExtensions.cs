namespace GridDomain.Node.Configuration {
    public static class NodeNetworkAddressExtensions
    {
        public static string ToFullTcpAddress(this INodeNetworkAddress conf, string name)
        {
            return $"akka.tcp://{name}@{conf.Host}:{conf.PortNumber}";
        }
    }
}