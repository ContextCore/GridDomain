using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace SchedulerDemo.Configuration
{
    internal class LocalAkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string SystemName => "LocalSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8080;
        public string PublicHost => Host;
    }
}