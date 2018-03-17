using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Node.Cluster
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
                                     .Log(conf.LogLevel)
                                     .ClusterSeed(conf, otherSeeds)
                                     .SqlPersistence(cfg)
                                     .BuildHocon();
        }

         public static string ToClusterNonSeedNodeSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, params INodeNetworkAddress[] seeds)
         {
             return ActorSystemBuilder.New()
                                      .Log(conf.LogLevel)
                                      .ClusterNonSeed(conf, seeds)
                                      .SqlPersistence(persistence)
                                      .BuildHocon();
         }

        public static string ToStandAloneSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, bool serializeMessagesCreators = false)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Address)
                                     .SqlPersistence(persistence)
                                     .BuildHocon();
        }

        
    }
}