using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Connector
{
    internal class ConsoleAkkaConfiguration : AkkaConfiguration
    {
        public ConsoleAkkaConfiguration()
            : base(new AkkaNetworkAddress("Connector", "localhost", 0), new ConsoleDbConfig()) {}
    }
}