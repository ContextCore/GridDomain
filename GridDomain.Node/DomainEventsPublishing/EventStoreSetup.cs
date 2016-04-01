using GridDomain.EventStore.MSSQL;
using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;

namespace GridDomain.Node.DomainEventsPublishing
{
    public static class EventStoreSetup
    {
        public static IStoreEvents WireupEventStore(string eventStoreConnectionString, params IPipelineHook[] hooks)
        {
            return Wireup.Init()
                //  .LogTo(t => new NLogToEventStoreLogAdapter(LogManager.GetLogger(t.Name).))
                .UsingSqlPersistence(new MsSqlConnectionFactory(eventStoreConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                //   .TrackPerformanceInstance("GridDomain")
                .UsingJsonSerialization()
                .Compress()
                //.EncryptWith(EncryptionKey)
                .HookIntoPipelineUsing(hooks)
                .Build();
        }
    }
}