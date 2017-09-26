using System;
using System.Linq;

namespace GridDomain.Node.Configuration.Hocon
{
    public class ClusterConfig : RemoteActorConfig
    {
        private readonly string[] _seedNodes;

        public ClusterConfig(INodeNetworkAddress config, params string[] seedNodes) : base(config)
        {
            _seedNodes = seedNodes;
        }

        private string BuildClusterProvider(params string[] seedNodes)
        {
            var seeds = string.Join(Environment.NewLine, seedNodes.Select(n => @"""" + n + @""""));

            var clusterConfigString = @"actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
            cluster {
                            seed-nodes = [" + seeds + @"]
            }";


            return clusterConfigString;
        }

        public static ClusterConfig SeedNode(INodeNetworkAddress address, params INodeNetworkAddress[] otherSeeds)
        {
            var allSeeds = otherSeeds.Union(new[] {address});
            var seedNodes = allSeeds.Select(GetSeedNetworkAddress).ToArray();
            return new ClusterConfig(address, seedNodes);
        }

        public static ClusterConfig NonSeedNode(INodeNetworkAddress address, INodeNetworkAddress[] seedNodesAddresses)
        {
            var seedNodes = seedNodesAddresses.Select(GetSeedNetworkAddress).ToArray();
            return new ClusterConfig(new NodeNetworkAddress(address.SystemName, address.Host, 0), seedNodes);
        }

        private static string GetSeedNetworkAddress(INodeNetworkAddress conf)
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