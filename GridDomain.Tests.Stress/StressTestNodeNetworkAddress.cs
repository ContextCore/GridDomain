using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Stress
{
    internal class StressTestNodeNetworkAddress : INodeNetworkAddress
    {
        public string Host => "127.0.0.1";
        public int PortNumber => 0;
        public bool EnforceIpVersion => true;
        public string PublicHost => Host;
    }
}