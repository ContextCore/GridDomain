using System;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Persistence.Sql
{
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemBuilder SqlPersistence(this ActorSystemBuilder builder, ISqlNodeDbConfiguration conf)
        {
            builder.Add(new PersistenceConfig(new PersistenceJournalConfig(conf ?? throw new ArgumentNullException(nameof(conf)),
                                                                           new DomainEventAdaptersConfig()),
                                              new PersistenceSnapshotConfig(conf)));
            return builder;
        }

        public static IActorSystemFactory Build(this ActorSystemBuilder builder, NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
            return builder.Log(conf.LogLevel)
                          .DomainSerialization(false)
                          .RemoteActorProvider()
                          .Remote(conf.Address)
                          .SqlPersistence(persistence)
                          .BuildActorSystemFactory(conf.Name);
        }
    }
}