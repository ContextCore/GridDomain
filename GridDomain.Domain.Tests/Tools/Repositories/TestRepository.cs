using GridDomain.Node;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.Tools
{
    public static class TestRepository
    {
        public static IRepository NewInMemory()
        {
            return new Repository(new AkkaEventRepository(new AutoTestAkkaConfiguration().CreateInMemorySystem()));
        }

        public static IRepository NewPersistent()
        {
            return new Repository(AkkaEventRepository.New(new AutoTestAkkaConfiguration()));
        }
    }
}