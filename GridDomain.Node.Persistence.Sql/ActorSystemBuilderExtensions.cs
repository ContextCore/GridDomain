using System;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Persistence.Sql
{
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemConfigBuilder SqlPersistence(this ActorSystemConfigBuilder configBuilder, ISqlNodeDbConfiguration conf)
        {
            configBuilder.Add(new PersistenceConfig(new PersistenceJournalConfig(conf ?? throw new ArgumentNullException(nameof(conf)),
                                                                           new DomainEventAdaptersConfig()),
                                              new PersistenceSnapshotConfig(conf)));
            return configBuilder;
        }

        public static IActorSystemFactory Build(this ActorSystemConfigBuilder configBuilder, NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
            return configBuilder.Log(conf.LogLevel)
                          .DomainSerialization(false)
                          .RemoteActorProvider()
                          .Remote(conf.Address)
                          .SqlPersistence(persistence)
                          .BuildActorSystemFactory(conf.Name);
        }
    }
}