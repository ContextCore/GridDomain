using GridDomain.Node.DomainEventsPublishing;
using NEventStore;
using NLog;

namespace GridDomain.Domain.Tests
{
    static internal class TestEventStoreSetup
    {
        public static IStoreEvents CreateMemoryEventStore()
        {
            return Wireup.Init()
                         .LogTo(t => new NLogToEventStoreLogAdapter(LogManager.GetLogger(t.Name)))
                         .UsingInMemoryPersistence()
                         .InitializeStorageEngine()
                         .UsingJsonSerialization()
                         .Build();
        }
    }
}