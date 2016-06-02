using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance
{
    internal class AutoTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8080;
    }
}