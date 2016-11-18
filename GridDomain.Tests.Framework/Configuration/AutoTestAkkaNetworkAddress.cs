using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    internal class AutoTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "127.0.0.1";
        public string PublicHost => Host;
        public int PortNumber => 8090;
        public bool EnforceIpVersion => true;
    }
}