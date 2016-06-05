using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using GridDomain.Tests.Configuration;

namespace GridDomain.Tests.Acceptance
{
    public static class TestEnvironment
    {
        public static readonly IDbConfiguration Configuration
            = new AutoTestLocalDbConfiguration();

        public static readonly IAkkaDbConfiguration AkkaConfiguration
            = new AutoTestAkkaDbConfiguration();
    }
}