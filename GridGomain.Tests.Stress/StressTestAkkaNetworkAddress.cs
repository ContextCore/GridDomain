using GridDomain.Node.Configuration.Akka;

namespace GridGomain.Tests.Stress
{
    internal class StressTestAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "StressTestSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8081;
        public string PublicHost => Host;
    }
}