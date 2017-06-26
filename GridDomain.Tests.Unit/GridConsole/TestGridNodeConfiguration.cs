using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Common.Configuration;

namespace GridDomain.Tests.Unit.GridConsole
{
    internal class TestGridNodeConfiguration : AkkaConfiguration
    {
        public TestGridNodeConfiguration()
            : base(new AkkaNetworkAddress("RemoteSystem", "127.0.0.1", 9000), new AutoTestAkkaDbConfiguration()) {}
    }
}