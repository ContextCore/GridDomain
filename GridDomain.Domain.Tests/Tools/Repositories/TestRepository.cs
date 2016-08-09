using GridDomain.Tools;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.Tools
{
    public static class TestRepository
    {
        public static IRepository NewInMemory()
        {
            return new Repository(EventRepositoryPresets.NewInMemory());
        }

        public static IRepository NewPersistent()
        {
            return new Repository(EventRepositoryPresets.NewPersistent());
        }
    }
}