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

        public static IActorSystemFactory Build(this ActorSystemBuilder builder, AkkaConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
            return builder.Log(conf.LogLevel, conf.LogActorType, false)
                          .DomainSerialization(false)
                          .RemoteActorProvider()
                          .Remote(conf.Network)
                          .SqlPersistence(persistence)
                          .BuildActorSystemFactory(conf.Network.SystemName);
        }
    }
}