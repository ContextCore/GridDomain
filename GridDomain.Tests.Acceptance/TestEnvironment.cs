using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;

namespace GridDomain.Tests.Acceptance
{
    public static class TestEnvironment
    {
        public static readonly IDbConfiguration Configuration 
                                    = new AutoTestLocalDbConfiguration();
    }
}