using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools;

namespace GridDomain.Tests.Tools
{
    public static class TestEventRepository
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