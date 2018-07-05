using System;
using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;
using GridDomain.Node.Logging;
using Serilog.Events;

namespace GridDomain.Node.Configuration {
    
    public static class ActorSystemConfigBuilderExtensions
    {
        public static IActorSystemConfigBuilder Log(this IActorSystemConfigBuilder builder, LogEventLevel verbosity, Type logActorType = null, bool writeConfig=false)
        {
            builder.Add(new LogConfig(verbosity, logActorType: logActorType ?? typeof(SerilogLoggerActor), configOnStart:writeConfig));
            return builder;
        }
        
        public static IActorSystemConfigBuilder Remote(this IActorSystemConfigBuilder builder, INodeNetworkAddress network)
        {
            builder.Add(new TransportConfig(network));
            return builder;
        }

        
        public static IActorSystemConfigBuilder Add(this IActorSystemConfigBuilder builder, Config cfg)
        {
            builder.Add(new CustomConfig(cfg));
            return builder;
        }

        public static IActorSystemConfigBuilder DomainSerialization(this IActorSystemConfigBuilder builder, bool serializeMessagesAndProps = false)
        {
            builder.Add(new SerializersConfig(serializeMessagesAndProps, serializeMessagesAndProps));
            return builder;
        }
        
        public static IActorSystemConfigBuilder RemoteActorProvider(this IActorSystemConfigBuilder builder)
        {
            builder.Add(new RemoteActorProviderConfig());
            return builder;
        }

        public static IActorSystemConfigBuilder InMemoryPersistence(this IActorSystemConfigBuilder builder)
        {
            builder.Add(new PersistenceConfig(new InMemoryJournalConfig(new DomainEventAdaptersConfig()),
                                              new LocalFilesystemSnapshotConfig()));
            return builder;
        }
    }
}