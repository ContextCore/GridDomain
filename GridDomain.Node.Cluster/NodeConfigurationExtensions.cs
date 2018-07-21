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
            return ConfigureCluster(new ActorSystemConfigBuilder(), conf.Name, workerNodes, otherSeeds)
                   .Build()
                   .Create();
        }

        public static IActorSystemConfigBuilder ToClusterConfig(this NodeConfiguration conf,
                                                                int workerNodes = 0,
                                                                params INodeNetworkAddress[] otherSeeds)
        {
            return ConfigureCluster(new ActorSystemConfigBuilder(), conf.Name, workerNodes, otherSeeds)
                   .Build()
                   .SeedNodes.First();
        }

        public static ClusterConfigBuilder ConfigureCluster(this IActorSystemConfigBuilder actorSystemConfigBuilder, string confName, int workerNodes = 0, params INodeNetworkAddress[] otherSeeds)
        {
            return actorSystemConfigBuilder
                   .Cluster(confName)
                   .Seeds(otherSeeds)
                   .AutoSeeds(2)
                   .Workers(2)
                   .Workers(workerNodes);
        }
    }
}