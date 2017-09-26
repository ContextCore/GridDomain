using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Unit.GridConsole
{
    internal class TestGridNodeConfiguration : NodeConfiguration
    {
        public TestGridNodeConfiguration(int port)
            : base(new NodeNetworkAddress("ServerSystem", "localhost", port), new AutoTestNodeDbConfiguration()) {}
    }
}