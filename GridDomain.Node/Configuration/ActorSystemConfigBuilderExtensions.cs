using System;
using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;
using GridDomain.Node.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    
    public static class ActorSystemConfigBuilderExtensions
    {
        public static ActorSystemConfigBuilder Log(this ActorSystemConfigBuilder builder, LogEventLevel verbosity, Type logActorType = null, bool writeConfig=false)
        {
            builder.Add(new LogConfig(verbosity, logActorType ?? typeof(SerilogLoggerActor), writeConfig));
            return builder;
        }

        public static ActorSystemConfigBuilder Remote(this ActorSystemConfigBuilder builder, INodeNetworkAddress network)
        {
            builder.Add(new TransportConfig(network));
            return builder;
        }

        
        public static ActorSystemConfigBuilder Add(this ActorSystemConfigBuilder builder, Config cfg)
        {
            builder.Add(new CustomConfig(cfg));
            return builder;
        }

        public static ActorSystemConfigBuilder DomainSerialization(this ActorSystemConfigBuilder builder, bool serializeMessagesAndProps = false)
        {
            builder.Add(new SerializersConfig(serializeMessagesAndProps, serializeMessagesAndProps));
            return builder;
        }
        
        public static ActorSystemConfigBuilder RemoteActorProvider(this ActorSystemConfigBuilder builder)
        {
            builder.Add(new RemoteActorProviderConfig());
            return builder;
        }

        public static ActorSystemConfigBuilder InMemoryPersistence(this ActorSystemConfigBuilder builder)
        {
            builder.Add(new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                              new LocalFilesystemSnapshotConfig()));
            return builder;
        }
    }
}