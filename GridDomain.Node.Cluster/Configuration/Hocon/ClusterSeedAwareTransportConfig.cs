using System;
using System.Linq;
using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon
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

            var clusterConfigString = @"akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
            if (seeds.Any())
            {
                clusterConfigString +=
                @"
                akka.cluster {
                    seed-nodes =  [" + seeds + @"]
                }
                ";
            }

            return clusterConfigString;
        }
     
    }
}