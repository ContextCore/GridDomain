using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Node.Cluster
{

    public static class NodeConfigurationExtensions
    {
        public static Task<ClusterInfo> ToCluster(this NodeConfiguration conf, int workerNodes = 0,
                                                  params INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel)
                                     .DomainSerialization()
                                     .Cluster(conf.Name)
                                     .Seeds(otherSeeds)
                                     .AutoSeeds(1)
                                     .Workers(workerNodes)
                                     .Build()
                                     .Create();
        }
        
        public static string ToClusterConfig(this NodeConfiguration conf, int workerNodes = 0,
                                                  params INodeNetworkAddress[] otherSeeds)
        {
            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel)
                                     .DomainSerialization()
                                     .Cluster(conf.Name)
                                     .Seeds(otherSeeds)
                                     .AutoSeeds(1)
                                     .Workers(workerNodes)
                                     .Build()
                                     .CreateConfigs()
                                     .First();
        }
      
    }
}