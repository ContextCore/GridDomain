using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Unit.GridConsole
{
    internal class TestGridNodeConfiguration : AkkaConfiguration
    {
        public TestGridNodeConfiguration(int port)
            : base(new AkkaNetworkAddress("ServerSystem", "localhost", port), new AutoTestAkkaDbConfiguration()) {}
    }
}