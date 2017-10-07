using System;
using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Persistence.Sql
{

    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateSystem(this NodeConfiguration conf, ISqlNodeDbConfiguration cfg)
        {
            return ActorSystem.Create(conf.Name, conf.ToStandAloneSystemConfig(cfg));
        }

        public static string ToClusterSeedNodeSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration cfg, params INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType)
                                     .ClusterSeed(conf, otherSeeds)
                                     .SqlPersistence(cfg)
                                     .BuildHocon();
        }

         public static string ToClusterNonSeedNodeSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, params INodeNetworkAddress[] seeds)
         {
             return ActorSystemBuilder.New()
                                      .Log(conf.LogLevel, conf.LogActorType)
                                      .ClusterNonSeed(conf, seeds)
                                      .SqlPersistence(persistence)
                                      .BuildHocon();
         }

        public static string ToStandAloneSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, bool serializeMessagesCreators = false)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Address)
                                     .SqlPersistence(persistence)
                                     .BuildHocon();
        }

        
    }
}