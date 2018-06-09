using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using GridDomain.Node.Actors.Logging;
using GridDomain.Node.Configuration.Hocon;
using GridDomain.Node.Logging;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    public class ActorSystemBuilder
    {
        public readonly ILogger Logger;

        public ActorSystemBuilder(ILogger log=null)
        {
            Logger = log ?? Serilog.Log.Logger;
        }

        public List<IHoconConfig> Configs { get; private set; } = new List<IHoconConfig>();

        public static ActorSystemBuilder New(ILogger log=null)
        {
            return new ActorSystemBuilder(log);
        }
        public void Add(IHoconConfig cfg)
        {
            Configs.Add(cfg);
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

        public ActorSystemBuilder Clone()
        {
            return new ActorSystemBuilder(Logger) {Configs = Configs.ToList()};
        }
    }
}