using GridDomain.Node;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.Tools.Repositories
{
    public static class TestRepository
    {
        public static AggregateRepository NewInMemory()
        {
            return new AggregateRepository(new ActorSystemEventRepository(new AutoTestAkkaConfiguration().CreateInMemorySystem()));
        }

        public static AggregateRepository NewPersistent()
        {
            return new AggregateRepository(ActorSystemEventRepository.New(new AutoTestAkkaConfiguration()));
        }
    }
}