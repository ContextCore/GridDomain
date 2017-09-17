using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Stress
{
    internal class StressTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "StressTestSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8081;
        public bool EnforceIpVersion => true;
        public string PublicHost => Host;
    }
}