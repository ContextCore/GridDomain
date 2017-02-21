using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;

namespace GridDomain.Tests.Acceptance.XUnit.GridConsole
{
    class TestGridNodeConfiguration : AkkaConfiguration
    {
        public TestGridNodeConfiguration() : base(
            new AkkaNetworkAddress("RemoteSystem","127.0.0.1",9000),
            new AutoTestAkkaDbConfiguration())
        {
        }
    }
}