using GridDomain.Node;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Tools;
using GridDomain.Tools.Repositories;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Tools
{
    [TestFixture]
    class Test_event_repository_tools_Persistent : Test_event_repositoty_tools
    {
        protected override IEventRepository CreateRepository()
        {
            return new AkkaEventRepository(new AutoTestAkkaConfiguration().CreateSystem());
        }
    }
}