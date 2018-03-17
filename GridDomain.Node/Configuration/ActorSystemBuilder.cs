using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration.Hocon;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    public class ActorSystemBuilder
    {
        private List<IHoconConfig> _configs = new List<IHoconConfig>();

        public static ActorSystemBuilder New()
        {
            return new ActorSystemBuilder();
        }
        public void Add(IHoconConfig cfg)
        {
            _configs.Add(cfg);
        }

        public ActorSystemBuilder Log(LogEventLevel verbosity, Type logActorType = null, bool writeConfig=false)
        {
            Add(new LogConfig(verbosity, logActorType ?? typeof(SerilogLoggerActor), writeConfig));
            return this;
        }

        public ActorSystemBuilder Remote(INodeNetworkAddress network)
        {
            Add(new TransportConfig(network));
            return this;
        }

        public IActorSystemFactory BuildActorSystemFactory(string systemName)
        {
            var hocon = new RootConfig(_configs.ToArray());
            var factory = new HoconActorSystemFactory(systemName, hocon.Build());
            return factory;
        }
        public string BuildHocon()
        {
            var hocon = new RootConfig(_configs.ToArray());
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

        public ActorSystemBuilder Clone()
        {
            return new ActorSystemBuilder {_configs = _configs.ToList()};
        }
    }
}