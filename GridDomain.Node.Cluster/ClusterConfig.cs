using System;
using System.Linq;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster
{
    public class ClusterConfig : TransportConfig
    {
        private readonly string[] _seedNodes;
     //   private readonly NodeConfiguration _nodeConfiguration;
        private readonly string _name;

        public ClusterConfig(NodeConfiguration conf, params string[] seedNodes) : this(conf.Name, conf.Address, seedNodes)
        {
            
        }
        public ClusterConfig(string name, INodeNetworkAddress address, params string[] seedNodes) : base(address)
        {
            _name = name;
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

        public static ClusterConfig SeedNode(NodeConfiguration address, params INodeNetworkAddress[] otherSeeds)
        {
            var allSeeds = otherSeeds.Union(new[] {address.Address});
            var seedNodes = allSeeds.Select(s => GetSeedNetworkAddress(address.Name,s)).ToArray();
            return new ClusterConfig(address, seedNodes);
        }

        public static ClusterConfig NonSeedNode(NodeConfiguration address, INodeNetworkAddress[] seedNodesAddresses)
        {
            var seedNodes = seedNodesAddresses.Select(s => GetSeedNetworkAddress(address.Name,s)).ToArray();
            return new ClusterConfig(address.Name, new NodeNetworkAddress(address.Address.Host, 0), seedNodes);
        }

        private static string GetSeedNetworkAddress(string name, INodeNetworkAddress conf)
        {
            string networkAddress = $"akka.tcp://{name}@{conf.Host}:{conf.PortNumber}";
            return networkAddress;
        }
    }
}