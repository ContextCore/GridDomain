using System;
using System.Linq;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster
{
    public class ClusterSeedAwareTransportConfig : IHoconConfig
    {
        private readonly string[] _seedNodeFullAddresses;

        public ClusterSeedAwareTransportConfig(params string[] seedNodeFullAddresses)
        {
            _seedNodeFullAddresses = seedNodeFullAddresses;
        }

        public string Build()
        {
            var seeds = string.Join(Environment.NewLine, _seedNodeFullAddresses.Select(n => @"""" + n + @""""));

            var clusterConfigString = @"actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
            if (seeds.Any())
            {
                clusterConfigString +=
                @"
                cluster {
                    seed-nodes =  [" + seeds + @"]
                }
                ";
            }

            return clusterConfigString;
        }
     
    }
}