using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    internal class AutoTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "localhost";
        public int PortNumber => 8080;
    }
}