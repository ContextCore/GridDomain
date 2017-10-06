using GridDomain.Node.Configuration;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Acceptance.GridConsole
{
    internal class TestGridAkkaConfiguration : AkkaConfiguration
    {
        public TestGridAkkaConfiguration(int port)
            : base(new NodeNetworkAddress("ServerSystem", "localhost", port)) {}
    }
}