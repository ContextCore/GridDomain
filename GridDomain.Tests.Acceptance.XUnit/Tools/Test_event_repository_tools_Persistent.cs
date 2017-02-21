using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.EventRepositories;

namespace GridDomain.Tests.Acceptance.XUnit.Tools
{
    [TestFixture]
    class Test_event_repository_tools_Persistent : Test_event_repositoty_tools
    {
        protected override IRepository<DomainEvent> CreateRepository()
        {
            return ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog());
        }
    }
}