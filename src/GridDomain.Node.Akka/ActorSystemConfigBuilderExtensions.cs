using Akka.Configuration;
using GridDomain.Node.Akka.Configuration;
using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka
{
    public static class ActorSystemConfigBuilderExtensions
    {
        public static IActorSystemConfigBuilder Remote(this IActorSystemConfigBuilder builder,
            NodeNetworkAddress network)
        {
            builder.Add(new RemoteConfig(network));
            return builder;
        }


        public static IActorSystemConfigBuilder Add(this IActorSystemConfigBuilder builder, Config cfg)
        {
            builder.Add(new CustomConfig(cfg));
            return builder;
        }

        public static IActorSystemConfigBuilder RemoteActorProvider(this IActorSystemConfigBuilder builder)
        {
            builder.Add(new RemoteActorProviderConfig());
            return builder;
        }

        public static IActorSystemConfigBuilder InMemoryPersistence(this IActorSystemConfigBuilder builder)
        {
            builder.Add(new PersistenceConfig(new InMemoryJournalConfig(),
                                              new LocalFilesystemSnapshotConfig()));
            return builder;
        }
    }
}