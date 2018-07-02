using System;
using System.Linq;
using Akka.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster.Configuration.Hocon
{
    public class ClusterActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""";
        }
    }

    public class ClusterSeedNodes : IHoconConfig
    {
        private readonly string[] _seedNodeFullAddresses;

        public ClusterSeedNodes(params string[] seedNodeFullAddresses)
        {
            _seedNodeFullAddresses = seedNodeFullAddresses;
        }

        public string Build()
        {
            var seeds = string.Join(Environment.NewLine, _seedNodeFullAddresses.Select(n => @"""" + n + @""""));

            if (seeds.Any())
            {
                return @"
                akka.cluster {
                    seed-nodes =  ["
                       + seeds
                       + @"]
                }
                ";
            }

            return "";
        }
    }
}