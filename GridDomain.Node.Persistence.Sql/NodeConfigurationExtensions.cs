using System;
using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Persistence.Sql
{

    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateSystem(this AkkaConfiguration conf, ISqlNodeDbConfiguration cfg)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneSystemConfig(cfg));
        }

        public static string ToClusterSeedNodeSystemConfig(this AkkaConfiguration conf, ISqlNodeDbConfiguration cfg, params INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType, false)
                                     .ClusterSeed(conf.Network, otherSeeds)
                                     .SqlPersistence(cfg)
                                     .BuildHocon();
        }

         public static string ToClusterNonSeedNodeSystemConfig(this AkkaConfiguration conf, ISqlNodeDbConfiguration persistence, params INodeNetworkAddress[] seeds)
         {
             return ActorSystemBuilder.New()
                                      .Log(conf.LogLevel, conf.LogActorType, false)
                                      .ClusterNonSeed(conf.Network, seeds)
                                      .SqlPersistence(persistence)
                                      .BuildHocon();
         }

        public static string ToStandAloneSystemConfig(this AkkaConfiguration conf, ISqlNodeDbConfiguration persistence, bool serializeMessagesCreators = false)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType, false)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Network)
                                     .SqlPersistence(persistence)
                                     .BuildHocon();
        }

        
    }
}