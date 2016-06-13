using System;
using System.Linq;

namespace GridDomain.Node.Configuration.Hocon
{
    public class ClusterConfig : ActorConfig
    {
        private readonly string[] _seedNodes;

        public ClusterConfig(IAkkaNetworkAddress config, params string[] seedNodes) : base(config)
        {
            _seedNodes = seedNodes;
        }

        private string BuildClusterProvider(params string[] seedNodes)
        {
            var seeds = string.Join(Environment.NewLine, seedNodes.Select(n => @"""" + n + @""""));

            var clusterConfigString =
                @"
            actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
            cluster {
                            seed-nodes = [" + seeds + @"]
            }
";


            return clusterConfigString;
        }

        public static ClusterConfig SeedNode(IAkkaNetworkAddress address, params IAkkaNetworkAddress[] otherSeeds)
        {
            var allSeeds = otherSeeds.Union(new[] {address});
            var seedNodes = allSeeds.Select(GetSeedNetworkAddress).ToArray();
            return new ClusterConfig(address, seedNodes);
        }


        public static ClusterConfig NonSeedNode(IAkkaNetworkAddress address, IAkkaNetworkAddress[] seedNodesAddresses)
        {
            var seedNodes = seedNodesAddresses.Select(GetSeedNetworkAddress).ToArray();
            return new ClusterConfig(new AkkaNetworkAddress(address.SystemName, address.Host, 0), seedNodes);
        }


        private static string GetSeedNetworkAddress(IAkkaNetworkAddress conf)
        {
            string networkAddress = $"akka.tcp://{conf.SystemName}@{conf.Host}:{conf.PortNumber}";
            return networkAddress;
        }

        public override string BuildActorProvider()
        {
            return BuildClusterProvider(_seedNodes);
        }
    }
}