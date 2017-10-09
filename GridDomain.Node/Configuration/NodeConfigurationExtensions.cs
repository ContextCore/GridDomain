using Akka.Actor;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration {
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Name, conf.ToStandAloneInMemorySystemConfig());
        }

        public static string ToStandAloneInMemorySystemConfig(this NodeConfiguration conf,bool serializeMessagesCreators = false)
        {
            var cfg = new RootConfig(new LogConfig(conf.LogLevel),
                                     new SerializersConfig(serializeMessagesCreators, serializeMessagesCreators),
                                     new RemoteActorProviderConfig(),
                                     new TransportConfig(conf.Address),
                                     new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                                           new LocalFilesystemSnapshotConfig()));

            return cfg.Build();
        }

    }
}