using System;
using System.Collections.Generic;
using Akka.Event;
using GridDomain.Node.Actors.Serilog;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration {
    public class ActorSystemBuilder
    {
        private List<IHoconConfig> Configs = new List<IHoconConfig>();

        public static ActorSystemBuilder New()
        {
            return new ActorSystemBuilder();
        }
        public void Add(IHoconConfig cfg)
        {
            Configs.Add(cfg);
        }

        public ActorSystemBuilder Log(LogLevel verbosity, Type logActorType = null, bool includeConfig = true)
        {
            Add(new LogConfig(verbosity, logActorType ?? typeof(SerilogLoggerActor), includeConfig));
            return this;
        }

        public ActorSystemBuilder Remote(INodeNetworkAddress network)
        {
            Add(new TransportConfig(network));
            return this;
        }

        public IActorSystemFactory BuildActorSystemFactory(string systemName)
        {
            var hocon = new RootConfig(Configs.ToArray());
            var factory = new HoconActorSystemFactory(systemName, hocon.Build());
            return factory;
        }
        public string BuildHocon()
        {
            var hocon = new RootConfig(Configs.ToArray());
            return hocon.Build();
        }
        public ActorSystemBuilder DomainSerialization(bool serializeMessagesAndProps = false)
        {
            Add(new SerializersConfig(serializeMessagesAndProps, serializeMessagesAndProps));
            return this;
        }
        public ActorSystemBuilder RemoteActorProvider()
        {
            Add(new RemoteActorProviderConfig());
            return this;
        }

        public ActorSystemBuilder InMemoryPersistence()
        {
            Add(new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                      new LocalFilesystemSnapshotConfig()));
            return this;
        }

        public ActorSystemBuilder ClusterSeed(INodeNetworkAddress thisSeed, params INodeNetworkAddress[] otherSeeds)
        {
            Add(ClusterConfig.SeedNode(thisSeed, otherSeeds));
            return this;
        }

        public ActorSystemBuilder ClusterNonSeed(INodeNetworkAddress thisSeed, params INodeNetworkAddress[] otherSeeds)
        {
            Add(ClusterConfig.NonSeedNode(thisSeed, otherSeeds));
            return this;
        }


    }
}