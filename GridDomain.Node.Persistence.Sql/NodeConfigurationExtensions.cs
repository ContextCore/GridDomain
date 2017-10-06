using System;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Persistence.Sql
{
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateSystem(this AkkaConfiguration conf)
        {
            return ActorSystem.Create(conf.Network.SystemName, conf.ToStandAloneSystemConfig());
        }

        public static string ToClusterSeedNodeSystemConfig(this AkkaConfiguration conf, params INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType, false)
                                     .ClusterSeed(conf.Network, otherSeeds)
                                     .SqlPersistence(conf.Persistence)
                                     .BuildHocon();
        }

         public static string ToClusterNonSeedNodeSystemConfig(this AkkaConfiguration conf, params INodeNetworkAddress[] seeds)
         {
             return ActorSystemBuilder.New()
                                      .Log(conf.LogLevel, conf.LogActorType, false)
                                      .ClusterNonSeed(conf.Network, seeds)
                                      .SqlPersistence(conf.Persistence)
                                      .BuildHocon();
         }

        public static string ToStandAloneSystemConfig(this AkkaConfiguration conf, bool serializeMessagesCreators = false)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel, conf.LogActorType, false)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Network)
                                     .SqlPersistence(conf.Persistence)
                                     .BuildHocon();
        }
    }
}