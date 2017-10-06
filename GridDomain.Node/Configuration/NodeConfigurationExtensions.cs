using Akka.Actor;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration {
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this AkkaConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneInMemorySystemConfig());
        }

        public static string ToStandAloneInMemorySystemConfig(this AkkaConfiguration conf,bool serializeMessagesCreators = false)
        {
            var cfg = new RootConfig(new LogConfig(conf.LogLevel, conf.LogActorType, false),
                                     new SerializersConfig(serializeMessagesCreators, serializeMessagesCreators),
                                     new RemoteActorProviderConfig(),
                                     new TransportConfig(conf.Network),
                                     new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                                           new LocalFilesystemSnapshotConfig()));

            return cfg.Build();
        }

    }
}