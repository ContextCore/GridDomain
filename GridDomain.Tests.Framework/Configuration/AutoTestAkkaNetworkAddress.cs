using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    internal class AutoTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8080;
    }
}