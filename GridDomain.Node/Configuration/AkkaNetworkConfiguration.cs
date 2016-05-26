namespace GridDomain.Node.Configuration
{
    public class AkkaNetworkConfiguration : IAkkaNetworkConfiguration
    {
        public string Name { get;}
        public string Host { get;}
        public int PortNumber { get; }

        public AkkaNetworkConfiguration(string name, string host, int port )
        {
            Name = name;
            Host = host;
            PortNumber = port;
        }
    }
}