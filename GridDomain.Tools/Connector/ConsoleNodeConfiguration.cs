using GridDomain.Node.Configuration;

namespace GridDomain.Tools.Connector
{
    internal class ConsoleNodeConfiguration : NodeConfiguration
    {
        public ConsoleNodeConfiguration()
            : base(new NodeNetworkAddress("Connector", "localhost", 0), new ConsoleDbConfig()) {}
    }
}