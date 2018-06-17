using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using GridDomain.Node.Cluster.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Node.Cluster
{
    public static class NodeConfigurationExtensions
    {
        public static Task<ClusterInfo> ToCluster(this NodeConfiguration conf,
                                                  int workerNodes = 0,
                                                  params INodeNetworkAddress[] otherSeeds)
        {
            return ConfigureCluster(conf, workerNodes, otherSeeds)
                   .Build()
                   .Create();
        }

        public static IActorSystemConfigBuilder ToClusterConfig(this NodeConfiguration conf,
                                                                int workerNodes = 0,
                                                                params INodeNetworkAddress[] otherSeeds)
        {
            return ConfigureCluster(conf, workerNodes, otherSeeds)
                   .Build()
                   .SeedNodes.First();
        }
        

        public static ClusterConfigBuilder ConfigureCluster(NodeConfiguration conf, int workerNodes, INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemConfigBuilder.New()
                                           .Log(conf.LogLevel)
                                           .DomainSerialization(true)
                                           .InMemoryPersistence()
                                           .Cluster(conf.Name)
                                           .Seeds(otherSeeds)
                                           .AutoSeeds(1)
                                           .Workers(workerNodes);
        }
    }
}