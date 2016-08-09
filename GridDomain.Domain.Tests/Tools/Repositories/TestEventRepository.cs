using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.Tools
{
    public static class EventRepositoryPresets
    {
        public static IEventRepository NewInMemory()
        {
            return new AkkaEventRepository(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig());
        }

        public static IEventRepository NewPersistent()
        {
            return new AkkaEventRepository(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig());
        }
    }
}