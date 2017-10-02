using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Common.Configuration
{
    public class AutoTestNodeNetworkAddress : INodeNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "127.0.0.1";
        public string PublicHost => Host;
        public int PortNumber => 0;
        public bool EnforceIpVersion => true;
    }
}