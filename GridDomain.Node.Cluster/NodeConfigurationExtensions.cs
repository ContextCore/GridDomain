using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Node.Cluster
{

    public static class NodeConfigurationExtensions
    {
       // public static string ToClusterSeedNodeSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration cfg, params INodeNetworkAddress[] otherSeeds)
       // {
       //     return ActorSystemBuilder.New()
       //                              .Log(conf.LogLevel)
       //                              .ClusterSeed(conf, otherSeeds)
       //                              .SqlPersistence(cfg)
       //                              .BuildHocon();
       // }
       //
       //  public static string ToClusterNonSeedNodeSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence, params INodeNetworkAddress[] seeds)
       //  {
       //      return ActorSystemBuilder.New()
       //                               .Log(conf.LogLevel)
       //                               .ClusterNonSeed(conf, seeds)
       //                               .SqlPersistence(persistence)
       //                               .BuildHocon();
       //  }
    }
}