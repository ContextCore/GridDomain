using GridDomain.Node.Configuration;

namespace GridDomain.Tools.Connector
{
    internal class ConsoleAkkaConfiguration : AkkaConfiguration
    {
        public ConsoleAkkaConfiguration()
            : base(new NodeNetworkAddress("Connector", "localhost", 0)) {}
    }
}